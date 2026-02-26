/**
 * MCP Server Demo — AIAEGH Course
 *
 * This file demonstrates a minimal Model Context Protocol (MCP) server.
 *
 * MCP uses a client-server architecture:
 *   - The CLIENT is an AI-powered app such as VS Code (GitHub Copilot agent mode).
 *   - The SERVER (this file) exposes tools, resources, and prompts.
 *   - Communication happens over a TRANSPORT layer.
 *
 * Transport choices:
 *   - StdioServerTransport: the client launches the server as a subprocess and
 *     communicates over stdin/stdout. Best for local development tools.
 *   - SSEServerTransport:   the server runs as an HTTP server and pushes events
 *     over Server-Sent Events. Best for remote or shared servers.
 *
 * For this demo we use StdioServerTransport because:
 *   1. It requires no network setup.
 *   2. VS Code can start and stop it automatically.
 *   3. It is the most common choice for local MCP servers.
 */

import { McpServer } from "@modelcontextprotocol/sdk/server/mcp.js";
import { StdioServerTransport } from "@modelcontextprotocol/sdk/server/stdio.js";
import { z } from "zod";

// ---------------------------------------------------------------------------
// 1. Create the MCP server
//    Give it a name and version that clients can use for identification.
// ---------------------------------------------------------------------------
const server = new McpServer({
  name: "mcp-server-demo",
  version: "1.0.0",
});

// ---------------------------------------------------------------------------
// 2. Register a tool
//
//    A tool is an action the AI model can ask the server to perform.
//    Each tool has:
//      - a name        (used by the AI to call the tool)
//      - a description (helps the AI decide WHEN to use the tool)
//      - input schema  (validated automatically by the SDK using Zod)
//      - a handler     (the function that runs when the tool is called)
// ---------------------------------------------------------------------------
server.tool(
  // Tool name — the AI will reference this when calling the tool.
  "get_joke",

  // Description — write this for the AI, not for humans.
  // Clear descriptions help the model infer when to use the tool.
  "Returns a programming joke. Call this tool whenever the user asks for a joke, wants to laugh, or needs cheering up.",

  // Input schema — define the parameters the tool accepts.
  {
    topic: z
      .string()
      .optional()
      .describe(
        'Optional topic for the joke, e.g. "JavaScript", "Python", "bugs".'
      ),
  },

  // Handler — called when the AI invokes this tool.
  async ({ topic }) => {
    // TODO: Replace with real implementation (e.g., fetch from an API or perform a calculation).
    const jokes = {
      javascript: "Why do JavaScript developers wear glasses? Because they don't C#.",
      python: "Why do Python programmers prefer snake_case? Because they can't camelCase their way out of indentation errors.",
      bugs: "A QA engineer walks into a bar and orders 0 beers, 1 beer, 99 beers, -1 beers, and 'beer'. The bartender panics.",
      default: "Why do programmers prefer dark mode? Because light attracts bugs!",
    };

    const key = topic?.toLowerCase();
    const joke =
      (key && jokes[key]) ||
      jokes.default;

    return {
      content: [
        {
          type: "text",
          text: joke,
        },
      ],
    };
  }
);

// ---------------------------------------------------------------------------
// 3. Connect to the transport and start the server
//
//    StdioServerTransport reads JSON-RPC messages from stdin and writes
//    responses to stdout. VS Code (or any MCP client) manages this process.
// ---------------------------------------------------------------------------
const transport = new StdioServerTransport();
await server.connect(transport);
