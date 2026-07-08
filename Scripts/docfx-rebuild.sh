#!/usr/bin/env bash
set -e

rm -rf DocFx/api
rm -rf DocFx/_site

docfx metadata DocFx/docfx.json
docfx build DocFx/docfx.json
