# FutureMUD website deployment

Provision a dedicated `futuremud-web` service account with write access only to `/var/lib/futuremud-web`. Install the service unit and Nginx virtual host from this folder, then store production values in `/etc/futuremud-web/environment` with mode `0640` and ownership `root:futuremud-web`.

Required environment values:

- `FutureMUD__PublishingTokenSha256`: lowercase or uppercase hexadecimal SHA-256 of the release bearer token.
- `FutureMUD__DataRoot=/var/lib/futuremud-web`
- `ASPNETCORE_ENVIRONMENT=Production`

The SSH deployment user should be restricted to `~/incoming` and a root-owned `deploy-futuremud-web` command. Permit that command to switch `/opt/futuremud-web/current` and restart only `futuremud-web.service`; do not give it general-purpose root access.

Configure the deployment environment with `FUTUREMUD_WEB_DEPLOY_HOST`, `FUTUREMUD_WEB_DEPLOY_USER`, `FUTUREMUD_WEB_DEPLOY_KEY`, and `FUTUREMUD_WEB_KNOWN_HOSTS` secrets. Populate `FUTUREMUD_WEB_KNOWN_HOSTS` from a host key verified out of band; the workflow deliberately does not trust an unauthenticated `ssh-keyscan` result.

ASP.NET Core data-protection keys are persisted beneath `/var/lib/futuremud-web/keys`. They protect framework anti-forgery and temporary-data cookies; keep this directory private to the service account and include it in host backups.

Cloudflare should proxy `futuremud.com` and `www.futuremud.com` with Full (strict) origin TLS. Disable caching for `/api/publishing/*` and `/health/*`. The origin Nginx limit is 34 MiB so a 32 MiB publishing chunk plus request overhead fits without permitting unbounded uploads.
