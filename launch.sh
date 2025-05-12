#!/bin/bash

deno run db:push
deno run -A --env-file src/main.ts