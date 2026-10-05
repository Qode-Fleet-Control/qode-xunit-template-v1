# Built by .github/workflows/deploy.yml and pushed to Artifact Registry.
#
# A JOB image: its default command runs the test suite and exits 0 when every test
# passes (non-zero otherwise). It serves no HTTP and never satisfies a $PORT health check.
#   - single stage on the SDK image: `dotnet test` needs the SDK at run time;
#   - the project files are copied and restored first, so the package layer is cached
#     until a dependency changes; the build happens at image build, the run only tests;
#   - runs as the image's built-in non-root `app` user ($APP_UID).
FROM mcr.microsoft.com/dotnet/sdk:10.0
ARG BUILD_ID=""
ENV BUILD_ID=$BUILD_ID DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /src
RUN chown app:app /src
USER $APP_UID
COPY --chown=app:app App.slnx global.json ./
COPY --chown=app:app src/App/App.csproj src/App/
COPY --chown=app:app tests/App.Tests/App.Tests.csproj tests/App.Tests/
RUN dotnet restore App.slnx
COPY --chown=app:app . .
RUN dotnet build App.slnx --no-restore -c Release
CMD ["dotnet", "test", "--solution", "App.slnx", "--no-build", "-c", "Release"]
