#!/usr/bin/env bash
set -euo pipefail

cd openc3-cosmos-fakesat

# Build the openc3-cosmos-fakesat gem and place it in ./gems
ROOT_DIR="$(cd "$(dirname "$0")" && pwd)"
cd "$ROOT_DIR"

gemspec_file=$(ls *.gemspec 2>/dev/null | head -n 1 || true)
if [ -z "$gemspec_file" ]; then
  echo "No .gemspec found in $ROOT_DIR"
  exit 1
fi

mkdir -p dist

echo "Building gem from $gemspec_file..."
gem build "$gemspec_file"

gem_file=$(ls *.gem 2>/dev/null | tail -n 1 || true)
if [ -z "$gem_file" ]; then
  echo "Gem build failed: no .gem file found"
  exit 1
fi

mv -f "$gem_file" ../gems/
echo "Built gems/$gem_file"
