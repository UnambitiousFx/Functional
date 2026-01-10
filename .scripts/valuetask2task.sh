#!/usr/bin/env sh

# Copy files from src/Functional/ValueTasks to src/Functional/Tasks
# and replace occurrences of "ValueTask" with "Task" inside the copied files.
# Supports a dry-run mode and a force flag to overwrite an existing destination.

set -eu

# Comma- or space-separated list of project paths to operate on.
# Example: PROJECT_PATHS="src/Functional src/Functional.AspNetCore"
PROJECT_PATHS="src/Functional src/Functional.AspNetCore/Mvc src/Functional.AspNetCore/Http test/Functional.Tests test/Functional.AspNetCore.Tests/Extensions/Http test/Functional.AspNetCore.Tests/Extensions/Mvc"
VALUETASK_DIRECTORY=ValueTasks
TASK_DIRECTORY=Tasks

usage() {
  cat <<EOF
Usage: $(basename "$0") [options]

Options:
  -n, --dry-run         Show what would be done without making changes
  -f, --force           Overwrite existing destination if it exists
  -p, --paths PATHS     Comma- or space-separated list of project paths
                        (default: src/Functional)
  -h, --help            Show this help message

Description:
  For each project path, copies the contents of
  <PROJECT>/$VALUETASK_DIRECTORY into <PROJECT>/$TASK_DIRECTORY,
  replaces the string "ValueTask" with "Task" in copied files, and
  renames files/directories containing "ValueTask" in their names.
EOF
  exit 0
}

DRY_RUN=0
FORCE=0

# Parse args
while [ "$#" -gt 0 ]; do
  case "$1" in
    -n|--dry-run)
      DRY_RUN=1
      shift
      ;;
    -f|--force)
      FORCE=1
      shift
      ;;
    -p|--paths)
      if [ $# -lt 2 ]; then
        echo "Error: --paths requires an argument" >&2
        usage
      fi
      # Allow comma-separated or space-separated list
      PROJECT_PATHS=$(printf "%s" "$2" | tr ',' ' ')
      shift 2
      ;;
    -h|--help)
      usage
      ;;
    --)
      shift
      break
      ;;
    *)
      echo "Unknown option: $1" >&2
      usage
      ;;
  esac
done

# Find git repository root
if git_root=$(git rev-parse --show-toplevel 2>/dev/null); then
  ROOT=$git_root
else
  echo "Error: not inside a git repository or git not available." >&2
  exit 2
fi

# Helper: show planned actions for one project path
plan_actions_for_path() {
  project_path=$1
  SRC="$ROOT/$project_path/$VALUETASK_DIRECTORY"
  DEST="$ROOT/$project_path/$TASK_DIRECTORY"

  if [ ! -d "$SRC" ]; then
    echo "  (skipping) source directory does not exist: $SRC"
    return
  fi

  echo "Project: $project_path"
  echo "  Copy files from: $SRC"
  echo "                 to: $DEST"
  echo
  echo "  Files to be copied:"
  find "$SRC" -type f -print | sed -e "s|^$SRC/||" | sed 's/^/    /'
  echo
  echo "  Text replacements inside files:"
  echo "    Replace all occurrences of 'ValueTask' with 'Task' in copied files"
  echo
  echo "  File/dir renames:"
  find "$SRC" -depth -name '*ValueTask*' -print | sed -e "s|^$SRC/||" | sed 's/^/    /' || true
  echo
}

# Perform the copy and updates for one project path
process_path() {
  project_path=$1
  SRC="$ROOT/$project_path/$VALUETASK_DIRECTORY"
  DEST="$ROOT/$project_path/$TASK_DIRECTORY"

  if [ ! -d "$SRC" ]; then
    echo "Warning: source directory does not exist: $SRC" >&2
    return
  fi

  # If not running a dry-run: require --force if destination exists and is not empty
  if [ "$DRY_RUN" -eq 0 ] && [ -d "$DEST" ] && [ "$(ls -A "$DEST")" ] && [ "$FORCE" -ne 1 ]; then
    echo "Error: destination directory exists and is not empty: $DEST" >&2
    echo "Use --force to overwrite." >&2
    exit 4
  fi

  if [ "$DRY_RUN" -eq 1 ]; then
    plan_actions_for_path "$project_path"
    return
  fi

  echo "Copying files from $SRC to $DEST..."
  mkdir -p "$DEST"
  cp -a "$SRC/." "$DEST/"

  echo "Replacing occurrences of 'ValueTask' with 'Task' in files..."
  if command -v grep >/dev/null 2>&1; then
    files_to_edit=$(grep -Irl "ValueTask" -- "$DEST" || true)
    if [ -n "$files_to_edit" ]; then
      echo "$files_to_edit" | while IFS= read -r f; do
        echo "  Updating: $f"
        sed -i 's/ValueTask/Task/g' "$f"
      done
    else
      echo "  No files needed content updates."
    fi
  else
    echo "Warning: grep not available, skipping content replacements." >&2
  fi

  echo "Renaming files and directories containing 'ValueTask' in their names..."
  find "$DEST" -depth -name '*ValueTask*' -print | while IFS= read -r path; do
    newpath=$(printf "%s" "$path" | sed 's/ValueTask/Task/g')
    echo "  Renaming: $path -> $newpath"
    mv "$path" "$newpath"
  done

  echo "Done for project: $project_path"
}

# Iterate over the requested project paths
for project in $PROJECT_PATHS; do
  process_path "$project"
done

exit 0