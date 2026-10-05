# xUnit.net template

Provisioned from [`Qode-Fleet-Control/fleet-template-v1`](https://github.com/Qode-Fleet-Control/fleet-template-v1) — the fleet
lifecycle contract (`bin/`, `fleet.conf`, `compose.yaml`, deploy workflows) with an
xUnit.net v3 test suite laid on top.

    App.slnx                     the solution (src + tests)
    global.json                  `dotnet test` uses Microsoft.Testing.Platform
    src/App/                     a small library: Calculator, Inventory (replace with yours)
    tests/App.Tests/             the xUnit.net v3 suite
    Dockerfile                   SDK image; builds at image build, CMD runs the suite
    compose.yaml                 the fleet's docker runtime (service `app`, no ports)

The suite shows `[Fact]`, `[Theory]` with `[InlineData]` and `[MemberData]`/`TheoryData`,
`Assert.Throws`, an async test using `TestContext.Current.CancellationToken`, and a shared
`IClassFixture<T>`.

**This repo is not a service.** It serves no HTTP: `START_CMD` and `DOCKER_START_CMD` are
empty, so `bin/run` installs/builds and stops there. The job is the test suite.

## Origin

Generated 2026-10-05 with the official generators, inside the official SDK image (.NET SDK
10.0.401) — `xunit3` comes from xUnit.net's own template pack, as its getting-started guide
teaches:

    docker run --rm -u $(id -u):$(id -g) -e HOME=/tmp -v "$PWD":/w -w /w \
      mcr.microsoft.com/dotnet/sdk:10.0 bash -c '
        dotnet new install xunit.v3.templates
        dotnet new sln -n App
        dotnet new classlib -n App -o src/App --framework net10.0
        dotnet new xunit3 -n App.Tests -o tests/App.Tests --framework net10.0
        dotnet add tests/App.Tests reference src/App
        dotnet sln add src/App tests/App.Tests'

## Running it

**On the fleet / with docker**

    bin/run                          # = docker compose build (no server to start)
    docker compose run --rm app      # runs the suite; exit 0 = all passed

**Without docker** (needs the .NET 10 SDK on PATH)

    dotnet test                      # restore, build, run the suite
    FLEET_RUNTIME=process bin/run    # = dotnet restore App.slnx && dotnet build App.slnx

| step | process runtime | docker runtime |
|---|---|---|
| install | `dotnet restore App.slnx` | — |
| build | `dotnet build App.slnx --no-restore` | `docker compose build` |
| run the job | `dotnet test` | `docker compose run --rm app` |

## Deviations from the stock generator output, and why

- `Class1.cs` and `UnitTest1.cs` replaced by a small library and a suite that actually
  exercises xUnit's features.
- Projects under `src/` and `tests/`, not the repo root: .NET writes build output to each
  project's `bin/`/`obj/`, which at the root would collide with the fleet's `bin/` scripts.
- Added: `Dockerfile`, `compose.yaml`, `.dockerignore`, a compact `.gitignore` (the stock
  `dotnet new gitignore` ignores every `bin/` — including the fleet's), `.env.example`,
  `fleet.conf`, `bin/`, `.github/workflows/`, `docs/fleet-lifecycle.md`.
- No NuGet lock file: the generators do not create one.

## Serving over HTTP

Fleet apps are served at the root of their own hostname. If you add an HTTP endpoint, listen
on `0.0.0.0:$PORT` and serve at `/`, then set `PORT`, `HEALTH_PATH`, `START_CMD` and
`DOCKER_START_CMD` in `fleet.conf` and publish the port in `compose.yaml`.

## Verified

**The docker runtime has NOT been verified yet.** On 2026-10-05 the shared docker host's disk
stayed at 0-5G free (under the 6G floor for a build) for over five hours, so `docker compose build`
was never run for this repo. Run the checks below once before trusting the image.

What did pass, inside `mcr.microsoft.com/dotnet/sdk:10.0` (.NET SDK 10.0.401): `dotnet test` →
14 tests, 14 passed.

Still to run: `docker compose build && docker compose run --rm app` (expect exit 0).
