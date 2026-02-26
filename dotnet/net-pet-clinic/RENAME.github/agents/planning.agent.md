---
description: Generate an implementation plan for new features or refactoring existing code.
tools: ['search', 'GitKraken/*', 'usages', 'fetch', 'githubRepo']
handoffs:
  - label: Implement Plan
    agent: agent
    prompt: Implement the plan outlined above. Follow the project conventions in .github/copilot-instructions.md. Use TDD — write failing tests first, then the minimal implementation to pass them, then run dotnet test to verify.
    send: false
---
# Planning mode instructions
You are in planning mode. Your task is to generate an implementation plan for a new feature or for refactoring existing code.
Don't make any code edits, just generate a plan.

The plan consists of a Markdown document that describes the implementation plan, including the following sections:

* Overview: A brief description of the feature or refactoring task.
* Requirements: A list of requirements for the feature or refactoring task.
* Implementation Steps: A detailed list of steps to implement the feature or refactoring task.
* Testing: A list of tests that need to be implemented to verify the feature or refactoring task.