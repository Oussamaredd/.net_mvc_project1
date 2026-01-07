# Architecture

## Overview
This is a classic **ASP.NET MVC 5** application with an Entity Framework 6 data layer.

## Core design choices
- MVC separation: controllers handle flow, views render UI, models represent domain
- EF Code First for data access
- `IPhotoSharingContext` interface to keep controller loosely coupled to the EF DbContext
- Attribute routing for clean URLs (`/photos/...`)

## Data layer
- `PhotoSharingContext : DbContext`
  - `DbSet<Photo> Photos`
  - `DbSet<Comment> Comments`

## Seeding
- `PhotoSharingInitializer : CreateDatabaseIfNotExists<PhotoSharingContext>`
  - Seeds sample photos + comments on first creation only

## Image storage
- Image file is stored directly in DB in `Photo.PhotoFile` (byte array)
- UI loads images via `/photos/{id}/image`, which returns `File(bytes, mimeType)`

## UI
- Bootstrap 5 via CDN
- Partial view `_PhotoGallery.cshtml` renders card grid (reusable)
- `_Layout.cshtml` provides consistent navigation + footer
