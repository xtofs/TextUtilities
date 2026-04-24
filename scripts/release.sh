#!/usr/bin/env bash

set -euo pipefail

usage() {
  echo "Usage: scripts/release.sh [alpha|beta|patch|minor]"
  exit 1
}

if [[ $# -ne 1 ]]; then
  usage
fi

bump_kind="$1"
if [[ "$bump_kind" != "alpha" && "$bump_kind" != "beta" && "$bump_kind" != "patch" && "$bump_kind" != "minor" ]]; then
  usage
fi

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
csproj="$repo_root/src/TextUtilities/TextUtilities.csproj"

if [[ ! -f "$csproj" ]]; then
  echo "Could not find project file at $csproj"
  exit 1
fi

if [[ -n "$(git -C "$repo_root" status --porcelain)" ]]; then
  echo "Working tree is not clean. Commit or stash changes before running this script."
  exit 1
fi

current_version="$(grep -oE '<Version>[^<]+' "$csproj" | sed 's/<Version>//' | head -n1)"
if [[ -z "$current_version" ]]; then
  echo "Could not read <Version> from $csproj"
  exit 1
fi

if [[ "$current_version" =~ ^([0-9]+)\.([0-9]+)\.([0-9]+)(-([0-9A-Za-z.-]+))?$ ]]; then
  major="${BASH_REMATCH[1]}"
  minor="${BASH_REMATCH[2]}"
  patch="${BASH_REMATCH[3]}"
  prerelease="${BASH_REMATCH[5]:-}"
else
  echo "Unsupported version format: $current_version"
  exit 1
fi

if [[ "$bump_kind" == "alpha" || "$bump_kind" == "beta" ]]; then
  next_core="$major.$minor.$patch"
  target_label="$bump_kind"

  if [[ -z "$prerelease" ]]; then
    prerelease_label="$target_label"
    prerelease_number=1
  elif [[ "$prerelease" =~ ^([0-9A-Za-z-]+)\.([0-9]+)$ ]]; then
    current_label="${BASH_REMATCH[1]}"
    current_number="${BASH_REMATCH[2]}"

    if [[ "$current_label" == "$target_label" ]]; then
      prerelease_label="$target_label"
      prerelease_number="$((current_number + 1))"
    else
      prerelease_label="$target_label"
      prerelease_number=1
    fi
  else
    echo "Unsupported prerelease format for $target_label bump: $current_version"
    echo "Expected prerelease like '-alpha.1' or '-beta.1'"
    exit 1
  fi

  next_version="$next_core-$prerelease_label.$prerelease_number"
else
  if [[ "$bump_kind" == "patch" ]]; then
    patch="$((patch + 1))"
  else
    minor="$((minor + 1))"
    patch=0
  fi

  next_core="$major.$minor.$patch"
  next_version="$next_core"

  if [[ -n "$prerelease" ]]; then
    if [[ "$prerelease" =~ ^([0-9A-Za-z-]+)\.[0-9]+$ ]]; then
      prerelease="${BASH_REMATCH[1]}.1"
    fi

    next_version="$next_core-$prerelease"
  fi
fi

escaped_current="$(printf '%s' "$current_version" | sed 's/[.[\*^$()+?{|]/\\&/g')"
escaped_next="$(printf '%s' "$next_version" | sed 's/[&/]/\\&/g')"
sed -E -i.bak "s|<Version>$escaped_current</Version>|<Version>$escaped_next</Version>|" "$csproj"
rm -f "$csproj.bak"

tag="v$next_version"

echo "Bumping version: $current_version -> $next_version"
echo "Running release commands:"
echo "  git add src/TextUtilities/TextUtilities.csproj"
echo "  git commit -m \"Bump version to $next_version\""
echo "  git tag -a $tag -m \"Release $tag\""
echo "  git push origin main"
echo "  git push origin $tag"

git -C "$repo_root" add src/TextUtilities/TextUtilities.csproj
git -C "$repo_root" commit -m "Bump version to $next_version"
git -C "$repo_root" tag -a "$tag" -m "Release $tag"
git -C "$repo_root" push origin main
git -C "$repo_root" push origin "$tag"

echo "Done."