---
name: SubagentDelegation
description: When to delegate tasks and work to subagents.
---

# What I do

## Sub-Agent Delegation Strategy
To conserve context space and token usage:
- **Break work into discrete tasks** - Each feature, module, or logical unit should be delegated to a sub-agent
- **Use Task tool for implementation** - Spawn sub-agents for writing code, tests, and documentation

### When to Delegate
- Implementing a new class or module
- Writing tests for a component
- Creating utility functions
- Building integrations
- Any task that can be clearly scoped and described

### When NOT to Delegate
- Quick fixes or one-line changes
- Reading/reviewing existing code
- Planning and architecture decisions
- Git operations and commits

### How to use subagents
Analyze task dependencies from exec plan
   - Delegate parallel tasks together ie use [{},{},{}] array of tasks to start agents concurrently
   - Delegate sequential tasks one at a time
   - Provide each sub-agent with explicit file constraints
   - Make the build subagent read docs/PHASE<N>_SPEC.md for a full specification overview so that it doesn't miss any critical requirements.
   - Make the build subagent read docs/TESTING_GUIDE.md so that it knows how to handle unit testing.

### SUB-AGENT DELEGATION:
To save context space and token usage, delegate implementation tasks to sub-agents:

1. Break work into discrete, well-scoped tasks
2. Use the Task tool to spawn sub-agents for:
   - Implementing classes/modules
   - Writing tests
   - Creating utilities
   - Building integrations
3. Provide sub-agents with:
   - Give sub-agents the relevant architecture docs
   - interface definitions
   - Make the build subagent read docs/PHASE<N>_SPEC.md for a full specification overview so that it doesn't miss any critical requirements.
   - Clear requirements and acceptance criteria
   - File paths where code should be written
   - Specification details
   - Execution plan
   - Make sure relevant events are logged with sensible log levels.
   - Integration testing, making sure it is called by or calling other modules when relevant and make sure by testing it so no errors remain.
4. Subagent must apply best practices for security, logging, clarity. Avoid security holes, sql injection vulnerabilities and other security issues. 
5. Review 
   - Are the requirements are met 
   - Test sub-agent output before committing
   - Check if the new feature integrates well 
   - Correct routes, relevant front-end navigation access, calls and jobs and other workflow chaining issues remain.

Example delegation:
```
Task: "Implement FileSystemConnector class in src/ingestion/filesystem.py.
Requirements:
- Implement DataConnector interface (see docs/ARCHITECTURE.md and docs/phase<N>_SPEC.md)
- Walk directories recursively
- Filter by file extension
- Extract metadata (size, modified date, path)
- Return IngestedDocument objects
- Include docstrings and type hints
- Handle errors gracefully"
```

## Example Prompts

### New feature
```
Implement the FileSystemConnector class according to the DataConnector interface.
It should:
- Walk a directory recursively
- Support filtering by file extension
- Extract basic metadata (size, modified date, path)
- Return IngestedDocument objects

Follow the patterns in docs/ARCHITECTURE.md.
```

### Utility function
```
Create a utility function for semantic chunking of text documents.
Requirements:
- Respect paragraph boundaries
- Target chunk size of ~500 tokens
- Overlap of ~50 tokens between chunks
- Return chunks with metadata (position, total chunks)
```

### Integration
```
Implement the ChromaDB storage layer.
It should:
- Initialize collections on startup
- Add documents with embeddings and metadata
- Query by similarity with filters
- Handle connection errors gracefully
```

## Example Workflow

Here's how the Build Agent should handle a multi-part implementation:

### 1. Receive task and plan
```
User: Implement the ingestion pipeline with filesystem and markdown connectors.

Agent thinks:
- This involves multiple components
- I'll break it into sub-tasks and delegate each
- After each component, I'll commit
```

### 2. Delegate to sub-agents
```
Task 1 (delegate): "Implement base DataConnector interface in src/ingestion/base.py..."
Task 2 (delegate): "Implement FileSystemConnector in src/ingestion/filesystem.py..."
Task 3 (delegate): "Implement MarkdownConnector in src/ingestion/markdown.py..."
Task 4 (delegate): "Write tests for connectors in tests/test_connectors.py..."
```

### 3. Commit after each logical unit
```bash
# After base interface is implemented:
git add src/ingestion/base.py
git commit -m "feat: add DataConnector base interface"

# After filesystem connector:
git add src/ingestion/filesystem.py
git commit -m "feat: add FileSystemConnector with recursive walking"

# After markdown connector:
git add src/ingestion/markdown.py
git commit -m "feat: add MarkdownConnector with frontmatter parsing"

# After tests:
git add tests/test_connectors.py
git commit -m "test: add unit tests for ingestion connectors"
```

### 4. Update PROGRESS.md, docs/SESSION_LOG.md, docs/FUTURE_IDEAS.md, DECISION_LOG.md
```
Document what was completed, decisions made, and commit references where relevant.
```

## Sub-Agent Task Template

When delegating to a sub-agent, use this template:

```
Implement [component name] in [file path].

CONTEXT:
[Relevant background from architecture docs]

REQUIREMENTS:
- [Specific requirement 1]
- [Specific requirement 2]
- [etc.]

INTERFACE:
[Any interfaces or base classes to implement]

PATTERNS TO FOLLOW:
[Reference existing code patterns]

DELIVERABLES:
- Working implementation with type hints
- Docstrings for public methods
- Error handling with meaningful messages
- [Any specific outputs needed]
```
