# Routes

This project uses **attribute routing** with `[RoutePrefix("photos")]`.

## Gallery
- `GET /photos`
  - Query string:
    - `q` (search term)
    - `sort` (latest | oldest | title | user)
    - `page` (pagination index)

Examples:
- `/photos`
- `/photos?q=forest`
- `/photos?sort=title&page=2`
- `/photos?q=ali&sort=user`

## Photo Details
- `GET /photos/{id}`
  - Example: `/photos/12`

## Image Endpoint
- `GET /photos/{id}/image`
  - Example: `/photos/12/image`

Used in views as the image `src`.

## Create
- `GET /photos/create`
- `POST /photos/create`

## Delete
- `GET /photos/{id}/delete`
- `POST /photos/{id}/delete`