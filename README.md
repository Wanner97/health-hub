# Health Hub

Health Hub is a personal software project that combines multiple services into one system for collecting, importing, storing, and visualizing health data.

The long-term goal is to build a system in which health data collected by different devices such as a smartphone or smartwatch can be transferred to a backend in a secure and automated way and then explored through a dedicated frontend.

At the moment, the project is still in active development. The whole workflow currently runs locally, and imports are still performed manually.

---

> ## Work in progress
>
> This repository reflects an unfinished learning project that is still evolving in terms of architecture, features, and data flow.
>  
> At the current stage:
> - the Android app exports health data locally as JSON
> - the backend imports and persists that data locally
> - the frontend visualizes the imported data from the local API
> - the full end-to-end automation is not implemented yet

Because the project is still changing, this repository currently does not include a full setup guide.

---

## Why I built this project

This project is primarily a learning and practice project for me.

By working on it, I hope to improve my JavaScript and React skills and gain experience in working with larger amounts of data in more complex and nested structures.

---

## Project Overview

Health Hub currently consists of three main parts:

### Android App
A native Android application reads selected health data from Health Connect and exports it as structured JSON files.

### Backend
An ASP.NET Core Web API imports exported data, validates it, persists it locally, and exposes endpoints for the frontend.

### Frontend
A React-based frontend consumes the backend API and visualizes imported health data in different dashboard and detail views.

At the moment, the flow is still local and manual:

1. data is read from the Android device
2. the export is created as JSON
3. the JSON is imported into the backend
4. the frontend reads the stored data from the local API

---

> [!CAUTION]
>
> This project handles personal health data.
>
> The Android app reads sensitive data from the device it is installed on, and the backend persists that data in a database. Anyone who wants to use or adapt this code should do so very carefully and with full awareness of the sensitivity of such information.
>
> I do **not** guarantee that this application is fully secure, especially while it is still in development.
> As long as everything runs locally and imports are handled manually, no security measures will be implemented.
>
> This repository represents a learning project, not a security-audited production system. It should therefore **not** be treated as a finished or production-ready solution for handling sensitive personal data.

For the same reason, this project should also not be understood as medical software or as a medically certified product.

---

## Current Direction

If the project is continued through to a more complete state, the intended direction is:

- secure and more automated transfer of device-collected data to the backend
- less manual handling in the import workflow
- continued expansion of supported health datasets
- improved frontend dashboards and visualizations
- a more mature overall architecture across all involved services

---

> [!NOTE]
>
> This repository is public primarily for documentation and portfolio purposes.
>
> It is meant to show how I approach architecture, data handling, service boundaries, and frontend/backend integration while learning and iterating on a real project.
>
> It should be understood as an evolving practice project, not as a finished production-ready product.

---

### Author

Claudio Wanner
