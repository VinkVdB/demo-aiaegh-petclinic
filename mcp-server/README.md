# MCP Server Demo

A minimal [Model Context Protocol](https://modelcontextprotocol.io/) server for the **AIAEGH** course (GH-300).

It demonstrates how to build an MCP server using the official
[`@modelcontextprotocol/sdk`](https://www.npmjs.com/package/@modelcontextprotocol/sdk) package
and expose a simple **tool** that GitHub Copilot (or any other MCP client) can call.

## Running with Docker (recommended)

Build the image once, then VS Code manages the container lifecycle automatically via the MCP configuration.

```bash
# From this directory:
docker compose build
```

That's it — VS Code will start and stop the container on demand.

## Running locally (alternative)

```bash
npm install
npm start
```

## Connecting to GitHub Copilot in VS Code

The server communicates over **stdio** — no port is opened or needed.

1. Copy `mcp.json.example` to `.vscode/mcp.json` in your workspace root
   (or open **MCP: Open User Configuration** from the Command Palette and paste the `"servers"` block).

2. Verify: Command Palette → **MCP: List Servers** — `joke-server` should show as **Running**.

3. In GitHub Copilot **Agent** mode, ask: *"Tell me a programming joke."*
   Copilot will call the `get_joke` tool and return the result.

## Project structure

```
server.js          ← MCP server (read this first!)
package.json       ← dependencies: express, @modelcontextprotocol/sdk
docker-compose.yml ← run via Docker
mcp.json.example   ← VS Code MCP configuration template
```

## Next steps

- Replace the hardcoded jokes in `server.js` with a real API call.
- Add more tools, resources, or prompts following the same pattern.
- See the [MCP SDK documentation](https://github.com/modelcontextprotocol/typescript-sdk) for advanced usage.
