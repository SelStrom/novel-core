#!/bin/bash
# fix-subagent-start.sh
# Logs subagent invocations for the speckit.fix-multi pipeline.
# Runs on subagentStart hook for fix-pipeline agents only (see matcher in hooks.json).

json_input=$(cat)
timestamp=$(date '+%Y-%m-%d %H:%M:%S')

mkdir -p .fix/agents

subagent_type=$(echo "$json_input" | python3 -c "import sys,json; print(json.load(sys.stdin).get('subagent_type','unknown'))" 2>/dev/null || echo "unknown")
task=$(echo "$json_input" | python3 -c "import sys,json; print(json.load(sys.stdin).get('task','')[:120])" 2>/dev/null || echo "")

echo "[$timestamp] START $subagent_type: $task" >> .fix/agents/pipeline.log

# Allow all fix-pipeline subagents
cat << EOF
{
  "permission": "allow"
}
EOF
