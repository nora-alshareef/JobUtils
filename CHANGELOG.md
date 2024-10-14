# Changelog

All notable changes to this project will be documented in this file.

## [1.0.0] - 2024-09-02

### Added

- Initial release of the BaseWorker class for .NET applications.
- Abstract class `BaseWorker` extending `BackgroundService` for background worker implementation.
- Features for:
    - Configurable activity with `IsActive` property.
    - Optional iteration timeout via `IterationTimeout`.
    - Built-in error handling and logging.
    - Configurable delays for normal operation and after errors.
    - Cancellation support for graceful shutdown.
    - Time unit flexibility for delays and timeouts, allowing for various configurations (seconds, minutes, hours,
      days).

### Usage Documentation

- Provided examples for:
    - Creating a worker class by inheriting from `BaseWorker`.
    - Configuring worker settings in `appsettings.json`.
    - Registering the worker in `Program.cs`.

### Key Concepts

- Detailed explanations of core methods and properties:
    - `ProcessAsync`: Core worker logic implementation.
    - `IsActive`: Controls worker execution dynamically.
    - `IterationTimeout`: Manages maximum duration for worker execution.
    - `GetDelayTime` and `GetErrorDelayTime`: Configurable delays for worker iterations.

### Changed

- Improved documentation clarity and structure for better usability.

### Fixed

- No known issues fixed in this release.

---

## Future Releases

- Planned enhancements for additional features and improved configuration options.