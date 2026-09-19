#!/usr/bin/env python3
"""Launch/connect to a local FutureMUD server and run in-game commands."""

from __future__ import annotations

import argparse
import os
import queue
import re
import socket
import subprocess
import sys
import threading
import time
from pathlib import Path

DEFAULT_PROVIDER = "MySql.Data.MySqlClient"
DEFAULT_CONNECTION_STRING = (
    "server=localhost;port=3307;database=demo_dbo;uid=futuremud;"
    "password=rpiengine2020;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=300000;"
)
READY_TEXT = "[SUCCESS] - MUD is now ready to connect"
ANSI_RE = re.compile(r"\x1b\[[0-9;?]*[ -/]*[@-~]")
PASSWORD_RE = re.compile(r"(?i)(password\s*=\s*)[^;\s]+")

IAC = 255
WILL = 251
WONT = 252
DO = 253
DONT = 254
SB = 250
SE = 240
END_IAC = 0


class OutputCollector:
    def __init__(self, proc: subprocess.Popen[str], secrets: list[str]) -> None:
        self.proc = proc
        self.secrets = secrets
        self.lines: list[str] = []
        self.queue: queue.Queue[str] = queue.Queue()
        self.thread = threading.Thread(target=self._read, daemon=True)
        self.thread.start()

    def _read(self) -> None:
        if self.proc.stdout is None:
            return
        for line in self.proc.stdout:
            clean = redact(line.rstrip("\r\n"), self.secrets)
            self.lines.append(clean)
            self.queue.put(clean)

    def wait_for(self, text: str, timeout: float) -> bool:
        deadline = time.time() + timeout
        while time.time() < deadline:
            if self.proc.poll() is not None:
                return any(text in line for line in self.lines)
            try:
                line = self.queue.get(timeout=0.25)
            except queue.Empty:
                continue
            if text in line:
                return True
        return False

    def tail(self, count: int = 80) -> str:
        return "\n".join(self.lines[-count:])

def windows_error_mode() -> int | None:
    """Suppress Windows crash dialogs for the MUD process and its children."""
    if os.name != "nt":
        return None

    try:
        import ctypes

        # SEM_FAILCRITICALERRORS | SEM_NOGPFAULTERRORBOX | SEM_NOOPENFILEERRORBOX
        return ctypes.windll.kernel32.SetErrorMode(0x0001 | 0x0002 | 0x8000)
    except (AttributeError, OSError):
        return None


def restore_windows_error_mode(previous_mode: int | None) -> None:
    if previous_mode is None or os.name != "nt":
        return

    try:
        import ctypes

        ctypes.windll.kernel32.SetErrorMode(previous_mode)
    except (AttributeError, OSError):
        pass

def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description="Drive a local FutureMUD telnet session.")
    parser.add_argument("--repo", default=os.getcwd(), help="FutureMUD repo root. Defaults to cwd.")
    parser.add_argument("--host", default="127.0.0.1", help="MUD host. Defaults to 127.0.0.1.")
    parser.add_argument("--port", type=int, default=4000, help="MUD port. Defaults to 4000.")
    parser.add_argument("--provider", default=DEFAULT_PROVIDER, help="Database provider argument.")
    parser.add_argument("--connection-string", default=None, help="Database connection string. Overrides FUTUREMUD_TEST_CONNECTION_STRING.")
    parser.add_argument("--account", default=None, help="Account name for login. Defaults by database for demo_dbo and labmud_dbo.")
    parser.add_argument("--password", default="password", help="Account password for login.")
    parser.add_argument("--character", default="1", help="Character selection input after account login.")
    parser.add_argument("--command", action="append", default=[], help="In-game command to run after login. Repeatable.")
    parser.add_argument("--commands-file", help="File containing in-game commands, one per line. # comments and blanks ignored.")
    parser.add_argument("--skip-login", action="store_true", help="Connect and run commands without sending the default login flow.")
    parser.add_argument("--use-running", action="store_true", help="Connect to an already-running MUD; do not build or launch.")
    parser.add_argument("--no-build", action="store_true", help="Skip dotnet build before launching.")
    parser.add_argument("--keep-running", action="store_true", help="Do not stop a MUD process launched by this script.")
    parser.add_argument("--startup-timeout", type=float, default=180.0, help="Seconds to wait for the MUD ready line during startup.")
    parser.add_argument("--connect-timeout", type=float, default=30.0, help="Seconds to wait for the telnet port after startup.")
    parser.add_argument("--step-delay", type=float, default=0.75, help="Delay after each login/menu input.")
    parser.add_argument("--read-after-command", type=float, default=2.0, help="Seconds to read output after each test command.")
    parser.add_argument("--expect", action="append", default=[], help="Literal text expected in the final transcript. Repeatable.")
    parser.add_argument("--expect-regex", action="append", default=[], help="Regex expected in the final transcript. Repeatable.")
    parser.add_argument("--transcript", help="Optional path to write the redacted transcript.")
    return parser.parse_args()


def redact(text: str, secrets: list[str]) -> str:
    text = PASSWORD_RE.sub(r"\1[REDACTED]", text)
    for secret in secrets:
        if secret:
            text = text.replace(secret, "[REDACTED]")
    return text


def strip_telnet(data: bytes) -> bytes:
    output = bytearray()
    i = 0
    while i < len(data):
        byte = data[i]
        if byte == END_IAC:
            i += 1
            continue
        if byte != IAC:
            output.append(byte)
            i += 1
            continue
        if i + 1 >= len(data):
            break
        command = data[i + 1]
        if command in (WILL, WONT, DO, DONT):
            i += 3
            continue
        if command == SB:
            i += 2
            while i + 1 < len(data) and not (data[i] == IAC and data[i + 1] == SE):
                i += 1
            i += 2
            continue
        i += 2
    return bytes(output)


def clean_output(data: bytes) -> str:
    text = strip_telnet(data).decode("iso-8859-1", errors="replace")
    text = ANSI_RE.sub("", text)
    text = text.replace("\x00", "")
    return text.replace("\r", "")


def get_connection_string(args: argparse.Namespace) -> str:
    return args.connection_string or os.environ.get("FUTUREMUD_TEST_CONNECTION_STRING") or DEFAULT_CONNECTION_STRING


def default_account_for_connection_string(connection_string: str) -> str | None:
    match = re.search(r"(?:^|;)\s*(?:database|initial\s+catalog)\s*=\s*([^;]+)", connection_string,
                      re.IGNORECASE)
    if match is None:
        return None

    database = match.group(1).strip().strip("'\"").casefold()
    return {
        "demo_dbo": "admin",
        "labmud_dbo": "japheth",
    }.get(database)


def load_commands(args: argparse.Namespace) -> list[str]:
    commands = list(args.command)
    if args.commands_file:
        path = Path(args.commands_file)
        for raw in path.read_text(encoding="utf-8-sig").splitlines():
            line = raw.strip()
            if not line or line.startswith("#"):
                continue
            commands.append(line)
    return commands


def ensure_repo(repo: Path) -> None:
    if not (repo / "MudSharpCore" / "MudSharpCore.csproj").is_file():
        raise SystemExit(f"Could not find MudSharpCore.csproj under repo: {repo}")


def run_build(repo: Path, secrets: list[str]) -> None:
    cmd = [
        "dotnet",
        "build",
        str(repo / "MudSharpCore" / "MudSharpCore.csproj"),
        "-c",
        "Debug",
        "--no-restore",
        "-m:1",
        "-p:NoWarn=NU1902%3BNU1510",
    ]
    print("== Building MudSharpCore ==")
    result = subprocess.run(cmd, cwd=str(repo), text=True, stdout=subprocess.PIPE, stderr=subprocess.STDOUT, errors="replace")
    if result.stdout:
        print(redact(result.stdout, secrets).rstrip())
    if result.returncode != 0:
        raise SystemExit(f"dotnet build failed with exit code {result.returncode}")


def launch_mud(repo: Path, runtime_dir: Path, args: argparse.Namespace, connection_string: str, secrets: list[str]) -> subprocess.Popen[str]:
    cmd = [
        "dotnet",
        "run",
        "--project",
        str(repo / "MudSharpCore" / "MudSharpCore.csproj"),
        "-c",
        "Debug",
        "--no-build",
        "--no-restore",
        "--",
        args.provider,
        connection_string,
    ]
    runtime_dir.mkdir(parents=True, exist_ok=True)
    print(f"== Launching MUD from {runtime_dir} ==")
    previous_error_mode = windows_error_mode()
    try:
        proc = subprocess.Popen(
            cmd,
            cwd=str(runtime_dir),
            stdout=subprocess.PIPE,
            stderr=subprocess.STDOUT,
            text=True,
            encoding="utf-8",
            errors="replace",
            creationflags=getattr(subprocess, "CREATE_NO_WINDOW", 0),
        )
    finally:
        restore_windows_error_mode(previous_error_mode)

    collector = OutputCollector(proc, secrets)
    if not collector.wait_for(READY_TEXT, args.startup_timeout):
        tail = collector.tail()
        log_tail = "\n".join(read_latest_console_log(runtime_dir, secrets).splitlines()[-80:])
        stop_process_tree(proc)
        raise SystemExit(
            f"MUD did not reach ready state within {args.startup_timeout:.0f}s. "
            f"Captured process output:\n{tail}\nConsole-log tail:\n{log_tail}"
        )
    print("== MUD is ready ==")
    return proc


def read_latest_console_log(runtime_dir: Path, secrets: list[str]) -> str:
    logs = sorted(runtime_dir.glob("Console Log *.txt"), key=lambda path: path.stat().st_mtime, reverse=True)
    for log in logs[:3]:
        try:
            return redact(log.read_text(encoding="utf-8", errors="replace"), secrets)
        except OSError:
            continue
    return ""


def wait_for_port(host: str, port: int, timeout: float) -> None:
    deadline = time.time() + timeout
    last_error: Exception | None = None
    while time.time() < deadline:
        try:
            with socket.create_connection((host, port), timeout=1.0):
                return
        except OSError as error:
            last_error = error
            time.sleep(0.25)
    raise SystemExit(f"Could not connect to {host}:{port} within {timeout:.0f}s: {last_error}")


class MudSocket:
    def __init__(self, host: str, port: int, secrets: list[str]) -> None:
        self.secrets = secrets
        self.sock = socket.create_connection((host, port), timeout=5.0)
        self.sock.settimeout(0.2)
        self.transcript: list[str] = []

    def close(self) -> None:
        try:
            self.sock.shutdown(socket.SHUT_RDWR)
        except OSError:
            pass
        self.sock.close()

    def read_for(self, seconds: float, until: str | None = None) -> str:
        deadline = time.time() + seconds
        chunks: list[bytes] = []
        while time.time() < deadline:
            try:
                data = self.sock.recv(8192)
            except socket.timeout:
                continue
            except OSError:
                break
            if not data:
                break
            chunks.append(data)
            text = clean_output(b"".join(chunks))
            if until and until.lower() in text.lower():
                break
        text = clean_output(b"".join(chunks))
        if text:
            clean = redact(text, self.secrets)
            self.transcript.append(clean)
            print(clean.rstrip())
        return text

    def send(self, command: str, *, secret: bool = False, read_seconds: float = 1.0) -> str:
        display = "[REDACTED]" if secret else command
        line = f"> {display}"
        self.transcript.append(line)
        print(line)
        self.sock.sendall(command.encode("iso-8859-1", errors="replace") + b"\r\n")
        time.sleep(0.05)
        return self.read_for(read_seconds)

    def full_transcript(self) -> str:
        return "\n".join(part.rstrip() for part in self.transcript if part is not None)


def stop_process_tree(proc: subprocess.Popen[str]) -> None:
    if proc.poll() is not None:
        return
    if os.name == "nt":
        subprocess.run(["taskkill", "/PID", str(proc.pid), "/T", "/F"], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    else:
        proc.terminate()
    try:
        proc.wait(timeout=10)
    except subprocess.TimeoutExpired:
        proc.kill()
        proc.wait(timeout=10)


def assert_expectations(transcript: str, args: argparse.Namespace) -> None:
    missing = [text for text in args.expect if text not in transcript]
    missing_regex = [pattern for pattern in args.expect_regex if not re.search(pattern, transcript, re.MULTILINE)]
    if missing or missing_regex:
        details: list[str] = []
        if missing:
            details.append("Missing expected text: " + ", ".join(repr(x) for x in missing))
        if missing_regex:
            details.append("Missing expected regex: " + ", ".join(repr(x) for x in missing_regex))
        raise SystemExit("; ".join(details))


def main() -> int:
    args = parse_args()
    repo = Path(args.repo).resolve()
    ensure_repo(repo)
    connection_string = get_connection_string(args)
    if args.account is None:
        args.account = default_account_for_connection_string(connection_string)
        if args.account is None:
            raise SystemExit("Pass --account when testing a database other than demo_dbo or labmud_dbo.")
    secrets = [connection_string, args.password]
    runtime_dir = repo / "MudSharpCore" / "bin" / "Debug" / "net10.0"
    commands = load_commands(args)

    proc: subprocess.Popen[str] | None = None
    transcript = ""
    try:
        if args.use_running:
            print("== Using already-running MUD ==")
        else:
            if not args.no_build:
                run_build(repo, secrets)
            proc = launch_mud(repo, runtime_dir, args, connection_string, secrets)

        wait_for_port(args.host, args.port, args.connect_timeout)
        session = MudSocket(args.host, args.port, secrets)
        try:
            session.read_for(2.0)
            if not args.skip_login:
                session.send("l", read_seconds=args.step_delay)
                session.send(args.account, read_seconds=args.step_delay)
                session.send(args.password, secret=True, read_seconds=args.step_delay)
                session.send("c", read_seconds=args.step_delay)
                session.send(args.character, read_seconds=max(2.0, args.step_delay))
            for command in commands:
                session.send(command, read_seconds=args.read_after_command)
            transcript = session.full_transcript()
        finally:
            session.close()

        assert_expectations(transcript, args)
        if args.transcript:
            Path(args.transcript).write_text(transcript, encoding="utf-8")
            print(f"== Wrote transcript to {args.transcript} ==")
        print("== MUD session completed ==")
        return 0
    finally:
        if proc is not None and not args.keep_running:
            print("== Stopping launched MUD process ==")
            stop_process_tree(proc)
            log_text = read_latest_console_log(runtime_dir, secrets)
            if READY_TEXT in log_text:
                print("== Confirmed ready line in console log ==")


if __name__ == "__main__":
    raise SystemExit(main())
