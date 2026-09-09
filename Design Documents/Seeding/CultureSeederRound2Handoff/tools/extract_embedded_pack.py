#!/usr/bin/env python3
"""Extract the checksum-verified ZIP embedded in the single-file Markdown handoff."""
from __future__ import annotations
import argparse, base64, hashlib, io, re, zipfile
from pathlib import Path, PurePosixPath

def extract(markdown: Path, destination: Path) -> Path:
    text=markdown.read_text(encoding='utf-8')
    match=re.search(r'<!-- ROUND2_ZIP_SHA256: ([0-9a-f]{64}) -->\s*<!-- ROUND2_ZIP_BASE64_BEGIN -->\s*```base64\s*([A-Za-z0-9+/=\s]+?)\s*```\s*<!-- ROUND2_ZIP_BASE64_END -->',text)
    if not match:raise ValueError('Embedded archive markers not found.')
    payload=base64.b64decode(''.join(match[2].split()),validate=True)
    if hashlib.sha256(payload).hexdigest()!=match[1]:raise ValueError('Embedded archive checksum mismatch.')
    destination=destination.resolve();destination.mkdir(parents=True,exist_ok=True)
    with zipfile.ZipFile(io.BytesIO(payload)) as archive:
        for info in archive.infolist():
            rel=PurePosixPath(info.filename)
            if rel.is_absolute() or '..' in rel.parts or '\\' in info.filename:raise ValueError('Unsafe ZIP path: '+info.filename)
            target=destination.joinpath(*rel.parts)
            if target.exists():raise FileExistsError(f'Refusing to overwrite existing content: {target}')
        archive.extractall(destination)
    return destination/'FutureMUD_CultureSeeder_Round2_Handoff'

def main()->None:
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('markdown',type=Path);parser.add_argument('destination',type=Path)
    args=parser.parse_args();print(extract(args.markdown,args.destination))
if __name__=='__main__':main()
