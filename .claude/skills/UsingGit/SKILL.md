---
name: UsingGit
description: Specific git usage in this project.
---

# What I do

## Git Workflow

### Commit Message Guidelines
- Keep under 72 characters
- Use imperative mood ("add" not "added")
- Focus on WHAT and WHY was added/changed
- Be specific but concise
- Examples:
  - `feat: add ChromaDB storage layer with similarity search`
  - `test: add unit tests for semantic chunking`
  - `fix: handle empty document edge case in ingestion`
  - `refactor: extract embedding logic to separate module`

GIT WORKFLOW:
After completing each logical unit of work:

1. Stage only the relevant changed files per work item
2. Commit with a concise, descriptive message
3. DO NOT push to remote - commits are for local tracking and review
4. Continue to next task - Commits create checkpoints for review

Commit message format: <type>: <brief description>
- Types: feat, fix, refactor, test, docs, chore
- Keep under 72 characters
- Use imperative mood ("add" not "added")

Examples:
- feat: add FileSystemConnector with recursive walking
- test: add unit tests for semantic chunking
- fix: handle empty document edge case
- refactor: extract embedding logic to module