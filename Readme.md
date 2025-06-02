# WordyTime

A .NET library for generating human-friendly, conversational descriptions of dates and times.

## Overview

**WordyTime** provides flexible and natural language formatting for `DateTime` values, enabling your application to display times and dates such as "just now", "in 5 minutes", "quarter past three", or "tomorrow". It aims to present output that reads more like how people actually talk about time.

## Features

- Converts DateTime values into conversational phrases.
- Supports both relative (e.g., "a minute ago", "in 2 hours") and absolute (e.g., "at 14:30", "half past three") formats.
- Handles different time zones.
- Includes support for special day expressions like "yesterday" and "tomorrow".
- Testable and adaptable via dependency injection with `IDateTimeProvider`.

## Usage

You can use the `ConversationalDateTimeFormatter` class to generate spoken-style descriptions for times:
