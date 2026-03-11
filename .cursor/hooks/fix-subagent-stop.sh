#!/bin/bash
# fix-subagent-stop.sh
# Logs subagent completion for the speckit.fix-multi pipeline.
# Runs on subagentStop hook for fix-pipeline agents only (see matcher in hooks.json).

json_input=$(cat)
timestamp=$(date '+%Y-%m-%d %H:%M:%S')

mkdir -p .fix/agents

subagent_type=$(echo "$json_input" | python3 -c "import sys,json; print(json.load(sys.stdin).get('subagent_type','unknown'))" 2>/dev/null || echo "unknown")
status=$(echo "$json_input" | python3 -c "import sys,json; print(json.load(sys.stdin).get('status','unknown'))" 2>/dev/null || echo "unknown")
duration=$(echo "$json_input" | python3 -c "import sys,json; print(json.load(sys.stdin).get('duration_ms',0))" 2>/dev/null || echo "0")
modified=$(echo "$json_input" | python3 -c "import sys,json; print(','.join(json.load(sys.stdin).get('modified_files',[])))" 2>/dev/null || echo "")

echo "[$timestamp] STOP  $subagent_type: status=$status duration=${duration}ms files=$modified" >> .fix/agents/pipeline.log

exit 0
