# FutureMUD website production deployment

This directory is the production baseline for the public website. It assumes Cloudflare proxies both public hostnames, Nginx is the only Internet-facing application process, ASP.NET Core listens only on loopback, and the deployment-only SSH exception is constrained by a forced-command gate.

The templates are not a substitute for checking the live Cloudflare zone, AWS account, security groups, host firewall, SSH daemon, installed .NET runtime, or Nginx configuration. Apply and verify all layers below. Never commit a real token, private key, origin address, account ID, zone ID, certificate, host key, or environment file.

## Security boundaries

- Cloudflare terminates public TLS and provides the public WAF, DDoS, and edge rate-limiting boundary.
- The EC2 security group accepts origin HTTPS on TCP/443 only from Cloudflare's published proxy ranges. Origin HTTP on TCP/80 is not permitted through the security group, and direct-to-origin HTTPS is denied.
- Authenticated Origin Pulls are required by Nginx as a second, independent check that an allowed HTTPS peer is Cloudflare.
- Nginx restores the visitor address only when the TCP peer is in a trusted Cloudflare range. It overwrites, rather than appends to, `X-Forwarded-For` before proxying to loopback.
- ASP.NET Core trusts forwarded headers only from loopback Nginx. Do not set `ASPNETCORE_FORWARDEDHEADERS_ENABLED`; the application has an explicit, narrower configuration.
- `futuremud-web` cannot write its deployed executable files. It can write only `/var/lib/futuremud-web`, and `UMask=0077` makes files it creates owner-only by default.
- `futuremud-deploy` owns only the upload directory. A root-owned forced-command gate denies a shell and permits only SFTP startup or one exact activation command; the privileged deploy helper then atomically claims and validates the archive.

## Host users and directories

Run the following once as an administrator on a new GNU/Linux host. The production host uses Amazon Linux 2023; Debian/Ubuntu path differences are called out below.

The helper expects GNU `coreutils`, `findutils`, `tar`, `gzip`, `util-linux` (`flock`), a POSIX `awk`, and `curl`. Do not substitute BSD implementations without rerunning the archive and rollback tests.

```bash
sudo useradd --system --user-group --home-dir /var/lib/futuremud-web \
  --shell /usr/sbin/nologin futuremud-web
sudo useradd --user-group --home-dir /var/lib/futuremud-web-deploy \
  --shell /bin/bash futuremud-deploy

sudo install -d -o futuremud-web -g futuremud-web -m 0750 /var/lib/futuremud-web
sudo install -d -o root -g futuremud-deploy -m 0750 /var/lib/futuremud-web-deploy
sudo install -d -o futuremud-deploy -g futuremud-deploy -m 0700 \
  /var/lib/futuremud-web-deploy/incoming
sudo install -d -o root -g root -m 0700 \
  /var/lib/futuremud-web-deploy/claimed
sudo install -d -o root -g futuremud-web -m 0750 \
  /opt/futuremud-web /opt/futuremud-web/releases
sudo install -d -o root -g futuremud-web -m 0750 /etc/futuremud-web
sudo install -d -o root -g root -m 0755 \
  /usr/local/libexec /etc/ssh/authorized_keys /etc/ssh/sshd_config.d \
  /etc/nginx/snippets /etc/nginx/certs /etc/nginx/conf.d
```

The deployment state parent and `/opt/futuremud-web` activation parent must remain root-owned and not group/world-writable. The helper verifies this because a deployment-user-controlled parent would reintroduce a pathname-swap vulnerability. `incoming` and `claimed` must be on the same filesystem so the helper can claim an uploaded inode with one atomic rename.

Existing installations that use `/home/futuremud-deploy/incoming` need a maintenance migration before the new helper is installed. Wait for all deployments to finish, create the root-owned state tree above, move the deployment account's home to `/var/lib/futuremud-web-deploy` in the account database, install its root-owned restricted `authorized_keys` file and command gate outside that writable home, and test both upload and activation. The workflow's relative `incoming/` destination does not change. Retain the old home read-only until one deployment and rollback test succeed, then remove it through the normal host change process.

Install the templates with root ownership. The final Nginx template enforces Authenticated Origin Pulls; if AOP is not already active, use the staged `optional` to `on` sequence in [Nginx, Origin CA, and Authenticated Origin Pulls](#nginx-origin-ca-and-authenticated-origin-pulls) instead of activating the final Nginx file immediately.

```bash
sudo install -o root -g root -m 0755 deploy-futuremud-web \
  /usr/local/sbin/deploy-futuremud-web
sudo install -o root -g root -m 0755 futuremud-deploy-gate \
  /usr/local/libexec/futuremud-deploy-gate
sudo install -o root -g root -m 0644 futuremud-web.service \
  /etc/systemd/system/futuremud-web.service
sudo install -o root -g root -m 0644 cloudflare-real-ip.conf \
  /etc/nginx/snippets/cloudflare-real-ip.conf
sudo install -o root -g root -m 0644 sshd-futuremud.conf \
  /etc/ssh/sshd_config.d/00-futuremud-hardening.conf

# Amazon Linux 2023
sudo install -o root -g root -m 0644 nginx-futuremud.conf \
  /etc/nginx/conf.d/futuremud.conf
```

On Debian/Ubuntu, install the Nginx template at `/etc/nginx/sites-available/futuremud.conf` and link it from `/etc/nginx/sites-enabled/futuremud.conf` instead. Do not install the same virtual hosts through both mechanisms. Disable any distribution default Nginx virtual host; another enabled `default_server` will either conflict with or weaken the explicit unknown-host rejection.

Prepare a file containing the deployment public key with the OpenSSH `restrict` option (for example, `restrict ssh-ed25519 ...`), verify its fingerprint independently, then install it outside the deployment user's writable home:

```bash
sudo install -o root -g root -m 0644 /secure/path/futuremud-deploy.authorized-key \
  /etc/ssh/authorized_keys/futuremud-deploy
sudo visudo -f /etc/sudoers.d/futuremud-web-deploy
```

The sudoers file must contain only this command grant:

```text
futuremud-deploy ALL=(root) NOPASSWD: /usr/local/sbin/deploy-futuremud-web
```

Confirm `/etc/ssh/sshd_config` includes `/etc/ssh/sshd_config.d/*.conf`, then validate `sshd -t` before reloading SSH. Keep the existing administrative session open until a second access path, preferably SSM Session Manager, has proved the new configuration.

## Application environment and publishing token

Create `/etc/futuremud-web/environment` with ownership `root:futuremud-web`, mode `0640`, and only these required production values:

```text
ASPNETCORE_ENVIRONMENT=Production
FutureMUD__DataRoot=/var/lib/futuremud-web
FutureMUD__PublishingTokenSha256=<64 hexadecimal SHA-256 characters>
```

Do not put the raw publishing bearer token on the host. Generate at least 32 random bytes on an administrator workstation, save the raw value directly into the GitHub `production` environment secret `FUTUREMUD_PUBLISHING_TOKEN`, and put only its SHA-256 digest in the host environment file. One suitable generation procedure is:

```bash
umask 077
openssl rand -hex 32 > futuremud-publishing-token
tr -d '\n' < futuremud-publishing-token | sha256sum
```

Transfer the raw value to the GitHub secret through an approved password-manager or secret-management path, then securely remove the temporary file. Do not paste it into chat, tickets, workflow output, shell history, cloud-init/user-data, or a GitHub repository variable.

The application currently accepts one publishing token. Rotate it at least every 90 days and immediately after suspected disclosure, maintainer departure, or unexpected publishing activity:

1. Temporarily disable or pause product publishing workflows.
2. Generate a new token and digest.
3. Replace the host digest, restart `futuremud-web.service`, and confirm `/health/ready` is healthy.
4. Replace `FUTUREMUD_PUBLISHING_TOKEN` in the protected GitHub `production` environment.
5. Run one controlled publish or authenticated preflight, then re-enable workflows.
6. Delete every copy of the previous token and review GitHub, Cloudflare, Nginx, and application logs for misuse.

Keep production environment approvals and branch/tag protection enabled. The publishing token can promote every configured product and must not be exposed to pull-request jobs or jobs that execute untrusted repository code.

ASP.NET Core data-protection keys are persisted under `/var/lib/futuremud-web/keys`. Keep the whole data root private to the service account and include it in encrypted, access-controlled backups. Test restoration. Do not copy production keys into development environments.

## Restrict the SSH deployment principal

The GitHub deployment environment uses `FUTUREMUD_WEB_DEPLOY_HOST`, `FUTUREMUD_WEB_DEPLOY_USER`, `FUTUREMUD_WEB_DEPLOY_KEY`, and `FUTUREMUD_WEB_KNOWN_HOSTS`. Verify the SSH host key through an independent console or AWS management channel before storing `KNOWN_HOSTS`; never trust an unauthenticated `ssh-keyscan` result by itself.

A deployment key must not provide a general shell, PTY, forwarding, agent forwarding, X11 forwarding, or arbitrary `sudo`. The current workflow requires both an OpenSSH SFTP transfer into `incoming/` and one activation command. `sshd-futuremud.conf` forces every deployment-key session through the root-owned `futuremud-deploy-gate`. The gate uses a fixed `PATH`, invokes SFTP only from one of the two fixed distribution paths, and permits activation only when `SSH_ORIGINAL_COMMAND` is exactly `deploy-futuremud-web '<40-lowercase-hex>'`. It never evaluates command text. Filesystem ownership leaves `incoming` as the only deployment-user-writable directory.

Grant passwordless elevation only for the root-owned deploy helper. Its own argument, directory, archive, ownership, size, disk-space, and rollback checks are security controls; do not replace it with broad `systemctl`, `tar`, `chown`, `rm`, shell, or wildcarded `/opt` permissions. Validate the effective deployment-user policy with `sshd -t` and `sshd -T -C user=futuremud-deploy,host=localhost,addr=127.0.0.1`.

The current no-cost workflow uses GitHub-hosted runners, whose egress addresses are not a small stable allowlist. TCP/22 open to `0.0.0.0/0` is therefore an explicitly accepted IPv4 residual, not a general administration path. Do not add `::/0`, keep password and keyboard-interactive authentication disabled, keep the key root-owned and forced through the gate, review authentication logs, and use SSM for administration. A fixed-egress runner or GitHub OIDC plus S3/SSM deployment would remove this residual later, but either changes the present workflow and may add cost or operational complexity.

Rotate the deployment key on the same incident and personnel-change triggers as the publishing token.

## Nginx and Cloudflare client addresses

`cloudflare-real-ip.conf` is a versioned snapshot of Cloudflare's official IPv4 and IPv6 proxy ranges. The site includes it before defining rate limits, so `$remote_addr` and `$binary_remote_addr` represent the visitor only when the connection actually came from a trusted Cloudflare peer. Each proxy location overwrites `X-Forwarded-For` with that address. Never change it back to `$proxy_add_x_forwarded_for`, and never trust `CF-Connecting-IP` from every source.

Install the list at `/etc/nginx/snippets/cloudflare-real-ip.conf`. Review it monthly and whenever Cloudflare announces an update:

```bash
curl --fail --silent --show-error https://www.cloudflare.com/ips-v4 > /tmp/cloudflare-ips-v4
curl --fail --silent --show-error https://www.cloudflare.com/ips-v6 > /tmp/cloudflare-ips-v6
sed -n 's/^set_real_ip_from \(.*\);$/\1/p' \
  /etc/nginx/snippets/cloudflare-real-ip.conf | sort -u > /tmp/nginx-cloudflare-ips
cat /tmp/cloudflare-ips-v4 /tmp/cloudflare-ips-v6 | sort -u > /tmp/current-cloudflare-ips
diff -u /tmp/nginx-cloudflare-ips /tmp/current-cloudflare-ips
```

Cloudflare's endpoints currently return one CIDR per line. If they differ, verify the change through Cloudflare's status/security communications. Add new ranges to both AWS/host firewalls and Nginx before Cloudflare begins using them; remove retired ranges only after the replacement is live. Update the verified date in the versioned file, deploy it, and run `nginx -t` before reload. Do not automate unaudited network content directly into production configuration.

## Nginx, Origin CA, and Authenticated Origin Pulls

The final template expects a Cloudflare Origin CA certificate at `/etc/ssl/certs/futuremud-origin.pem`, its private key at `/etc/ssl/private/futuremud-origin.key`, and Cloudflare's public Authenticated Origin Pull CA at `/etc/nginx/certs/cloudflare-origin-pull-ca.pem`. Install the origin private key as `root:root` mode `0600`; the two public certificates may be `root:root` mode `0644`. An Origin CA certificate is intentionally not browser-trusted and is valid here only because Cloudflare connects in `Full (strict)` mode. Track its expiry and rotate it before expiration.

Avoid an origin outage when enabling AOP:

1. Install the origin certificate, private key, AOP CA, and trusted proxy-range file. Verify the AOP CA against Cloudflare's official documentation.
2. Make a temporary copy of `nginx-futuremud.conf` with only `ssl_verify_client on;` changed to `ssl_verify_client optional;`, install that copy at the active distribution path, run `nginx -t`, reload Nginx, and verify the site through Cloudflare. `optional` is transition-only and must not be the final state.
3. Enable global Authenticated Origin Pulls in Cloudflare and verify the public site again.
4. Install the checked-in final template with `ssl_verify_client on;`, run `nginx -t`, reload, and verify the public site. A direct local TLS request without a Cloudflare client certificate should now receive HTTP 400.
5. Keep the Cloudflare CIDR security-group allowlist in place. Universal AOP authenticates Cloudflare's client certificate, not a particular customer zone.

Cloudflare publishes the AOP CA at <https://developers.cloudflare.com/ssl/static/authenticated_origin_pull_ca.pem>. Download it over verified TLS to a temporary file, inspect it, then install it; do not pipe live network content directly into active Nginx configuration.

The Nginx template deliberately:

- allows a 1 MiB request body by default and 34 MiB only beneath `/api/publishing/`;
- gives slow publishing uploads longer body/upstream timeouts while keeping public and health timeouts short;
- emits one one-year HSTS header with `includeSubDomains` and no `preload` on every FutureMUD HTTPS response;
- hides the application's HSTS header and replaces API/health cache headers so duplicates cannot weaken policy;
- rejects unknown HTTP and HTTPS hostnames;
- requires Cloudflare's Authenticated Origin Pull client certificate on every origin HTTPS virtual host;
- disables the Nginx version token; and
- overwrites the security-sensitive `Host`, visitor-address, and forwarded-scheme headers before proxying.

Do not add HSTS preload until every present and future subdomain is HTTPS-only and the owner explicitly accepts the long-lived browser commitment.

## Cloudflare production checklist

These settings require a logged-in Cloudflare review; they cannot be proven from this repository:

- Proxy both `futuremud.com` and `www.futuremud.com` (orange-cloud), remove stale DNS records, enable DNSSEC, and monitor certificate transparency.
- Use SSL/TLS mode `Full (strict)`, minimum edge TLS 1.2, TLS 1.3 enabled, and 0-RTT disabled for replay safety. Keep automatic HTTPS rewrites/redirects consistent with the canonical Nginx redirect.
- Enable Authenticated Origin Pulls after installing and testing the Cloudflare client CA in Nginx. Origin IP allowlisting remains required; universal AOP proves Cloudflare, not a particular customer zone.
- Enable the appropriate Cloudflare managed WAF rules, DDoS protections, bot controls, and rate limits. Publishing endpoints should have stricter rate/abuse monitoring than public downloads, but rules must not cache or alter authenticated upload requests.
- Create cache rules that bypass cache for `/api/publishing/*` and `/health/*`. Do not use Cache Everything on those paths. Preserve immutable download caching only where the application marks it safe.
- Ensure Cloudflare does not add a second HSTS or conflicting Cache-Control header. Let the reviewed origin policy pass through unchanged.
- Require phishing-resistant MFA for administrators, use least-privilege scoped API tokens rather than the Global API Key, restrict token source addresses where practical, and review members, service tokens, sessions, and audit logs.
- Store no Cloudflare credential in the repository, application environment, AMI, shell profile, or general deployment user. The public AOP CA and origin certificate are not secrets; the origin private key remains root-only.

The template uses a Cloudflare Origin CA certificate rather than public HTTP-01 validation. Keep the origin private key root-only, record the Origin CA expiry, test the replacement certificate before cutover, and retain the prior certificate only long enough to prove rollback.

## AWS and EC2 production checklist

Review these controls in the live AWS account:

- Security-group ingress permits TCP/443 only from the current Cloudflare IPv4 and IPv6 proxy CIDRs. Do not expose origin TCP/80 or Kestrel TCP/5070.
- The accepted GitHub-hosted-runner residual is TCP/22 from `0.0.0.0/0` only. Do not add `::/0`; enforce the root-owned key, forced-command gate, key-only authentication, and logging described above. Use SSM Session Manager for administration.
- If a host firewall is enabled, mirror the same ingress policy and keep it synchronized in the safe add-new/remove-old order used for Cloudflare range updates. The security group remains the required AWS network boundary.
- Require IMDSv2 (`HttpTokens=required`), set hop limit 1, and disable instance metadata tags. The standard `AmazonSSMManagedInstanceCore` instance role may remain attached so the SSM agent can operate; the website service separately denies both IPv4 and IPv6 metadata endpoints with systemd `IPAddressDeny`.
- Enable EBS encryption by default and block public EBS snapshots. Encryption by default affects new volumes, not an existing unencrypted root volume; migrating that volume and implementing tested backups remain explicit recovery work rather than a claim made by this baseline.
- Patch the OS, Nginx, OpenSSH, SSM agent, and the installed ASP.NET Core runtime promptly. This deployment is framework-dependent, so a repository rebuild alone does not update the host runtime. Verify `dotnet --list-runtimes` after patching.
- Review NACLs, route tables, elastic/public IPs, IPv6 assignment, outbound access, and every other security group attached to the ENI. A restrictive group is ineffective if another attached group permits broad ingress.
- Do not place secrets in EC2 user data, launch templates, tags, AMIs, snapshots, console screenshots, or world-readable files. Review `/etc/futuremud-web/environment`, certificate permissions, `authorized_keys`, `sudoers`, and shell histories.
- CloudTrail, GuardDuty/Security Hub, retained central logs, backups, and alarms should be risk-assessed separately; do not imply that paid controls are enabled as part of this no-cost baseline.

Restricting the origin to Cloudflare ranges is essential because an attacker who learns a historical origin address can otherwise bypass Cloudflare WAF, rate limits, bot controls, and edge TLS policy.

## Deploy-helper behavior and recovery

The helper serializes deployments with `flock` in a private root-owned `/run` directory, validates one exact commit SHA, and atomically moves the corresponding upload out of the deployment user's writable directory. It accepts only regular single-link archives owned by `futuremud-deploy`, seals a byte-for-byte root-created copy so an uploader-held file descriptor cannot race validation, caps compressed size at 512 MiB, permits only regular files/directories, caps 20,000 entries and 1 GiB expanded content, bounds archive inspection/extraction time, requires the archive size plus 256 MiB for validation, and retains a 1 GiB extraction-volume reserve.

A release is extracted into a new root-owned directory, checked for special files and the required website runtime files, permissioned read-only to the service, then activated by atomic symlink replacement. Any failure or signal after activation restores the prior symlink before deleting the failed release. A successful deploy keeps the active/rollback releases and the newest releases up to the helper's bounded retention policy. Failed uploads are consumed; re-upload before retrying.

Do not invoke the helper concurrently by bypassing its lock, edit `current` manually during deployment, or make release directories service-writable. For manual rollback, take the same deployment lock, select a validated 40-hex directory beneath `/opt/futuremud-web/releases`, replace `current` atomically, restart, and verify readiness before removing anything.

## Installation and verification

Validate templates before making them active:

```bash
sh -n /usr/local/sbin/deploy-futuremud-web
sh -n /usr/local/libexec/futuremud-deploy-gate
sudo nginx -t
sudo systemd-analyze verify /etc/systemd/system/futuremud-web.service
sudo sshd -t
sudo sshd -T -C user=futuremud-deploy,host=localhost,addr=127.0.0.1
sudo systemctl daemon-reload
sudo systemctl enable futuremud-web.service nginx
```

After the first deployment, verify the service and listening sockets:

```bash
sudo systemctl restart futuremud-web.service
sudo systemctl reload nginx
sudo systemctl --no-pager --full status futuremud-web.service nginx
sudo systemd-analyze security futuremud-web.service
sudo ss -lntp
sudo journalctl -u futuremud-web.service -n 100 --no-pager
```

Nginx may bind local TCP/80 for explicit default/canonical behavior, but the EC2 security group must expose only TCP/443 from Cloudflare. Kestrel must listen on `127.0.0.1:5070` only. Investigate any wildcard/public Kestrel listener.

Test the AOP boundary locally without exposing the origin address, then test application behavior through Cloudflare:

```bash
curl --insecure --silent --output /dev/null --write-out '%{http_code}\n' \
  --resolve futuremud.com:443:127.0.0.1 https://futuremud.com/health/ready
curl --fail --silent --show-error https://futuremud.com/health/ready
curl --silent --show-error --head https://futuremud.com/ \
  | grep -iE 'strict-transport-security|server|cache-control'
```

The direct request without a Cloudflare client certificate should return `400`; the public edge request should succeed. There should be exactly one HSTS header on FutureMUD HTTPS responses, no Nginx version, and one `Cache-Control: no-store` on API/health responses. Also verify unauthenticated publishing calls return `401`, public routes do not emit CORS allow headers, and oversized publishing requests are rejected.

Finally test from outside AWS:

- HTTP and `www` redirect once to canonical HTTPS.
- TLS 1.0/1.1 fail at Cloudflare and TLS 1.2/1.3 succeed.
- Direct origin 80/443 are unreachable from a non-Cloudflare address. IPv6 TCP/22 is closed; the accepted IPv4 TCP/22 residual permits only key authentication and the forced deployment gate.
- Cloudflare does not cache API/health responses, the visitor IP appears correctly in Nginx logs, and rate limits distinguish different visitors.
- A controlled deployment activates, a deliberately unhealthy package rolls back, old releases are pruned without touching active/rollback targets, and disk alarms fire before the configured reserve is consumed.

Repeat the Cloudflare, AWS, host, dependency/runtime, secret, and access review after material infrastructure changes and at least quarterly.

## Authoritative references

- Cloudflare proxy ranges: <https://www.cloudflare.com/ips/>
- Cloudflare origin protection: <https://developers.cloudflare.com/fundamentals/security/protect-your-origin-server/>
- Cloudflare Authenticated Origin Pulls: <https://developers.cloudflare.com/ssl/origin-configuration/authenticated-origin-pull/>
- Cloudflare minimum TLS: <https://developers.cloudflare.com/ssl/edge-certificates/additional-options/minimum-tls/>
- AWS EC2 exposure guidance: <https://docs.aws.amazon.com/securityhub/latest/userguide/exposure-ec2-instance.html>
