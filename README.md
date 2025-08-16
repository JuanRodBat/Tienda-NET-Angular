# Tienda — .NET 8 + Angular + SQL Server

Prueba técnica con **ASP.NET Core 8 (Web API)**, **SQL Server** y **Angular (standalone + Angular Material + Animations)**.
Incluye autenticación **JWT**, **CRUDs** de Tiendas/Artículos/Clientes, **stock por tienda**, **carrito de compras** y **historial de compras**.

## Stack

* **Backend**: .NET 8, Entity Framework Core (SQL Server), Swagger/OpenAPI, JWT Bearer.
* **DB**: SQL Server (Express/LocalDB/instancia local).
* **Frontend**: Angular 17/18 (standalone), Angular Material, HttpClient, Router.

---

## Arquitectura

```
/api/                         # Solución .NET
  Company.Project.Entitys/
  Company.Project.Data/
  Company.Project.Front/      # Web API
/spa/tienda-app/              # Angular (standalone + Material + Animations)
/db/                          # scripts SQL de esquema/seed
```

### Esquema de BD (mínimo)

* **Clientes**: Id, Nombre, Apellidos, Dirección, Email (unique), PasswordHash, FechaRegistro
* **Tiendas**: Id, Sucursal, Dirección
* **Articulos**: Id, Codigo (unique), Descripcion, Precio, Imagen
* **ArticuloTienda** *(relación artículo–tienda)*: **(ArticuloId, TiendaId)**, Stock, Fecha
* **ClienteArticulo** *(historial de compra)*: **(ClienteId, ArticuloId, Fecha)**, Cantidad
* (Extra) **vwCatalogoPorTienda**: vista para catálogo por tienda

> Nota: En EF Core se fuerza el nombre de tablas con `ToTable("...")` para evitar desajustes (p.ej. `ArticuloTienda`).

---

## Configurar y correr el **Backend**

1. **Cadena de conexión**
   Edita `api/Company.Project.Front/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=MI_PC\\MI_INSTANCIA;Database=TiendaDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": { "Key": "cambia-esta-clave-super-secreta-de-al-menos-32-chars" }
}
```

2. **Levantar la API**

```bash
cd api
dotnet build
dotnet run --project Company.Project.Front
```

La API expone Swagger en: `https://localhost:<PUERTO>/swagger`

3. **Primeros pasos en Swagger**

* Registrar cliente: `POST /api/auth/register`
* Login: `POST /api/auth/login` → copia el `token`
* Click en **Authorize** (candado) → pega **solo** el token (sin `Bearer `)

> **Claves JWT**: deben tener ≥16 bytes (≥128 bits). Si usas una frase corta, hashea a 256 bits o usa una cadena larga.

---

## Endpoints principales (API)

* **Ping / Diagnóstico**

  * `GET /api/ping/whoami` → servidor/BD actual
  * `GET /api/ping/db` → test de conexión
* **Tiendas**

  * `GET /api/tiendas` – `GET /{id}` – `POST` – `PUT /{id}` – `DELETE /{id}`
* **Artículos**

  * `GET /api/articulos` – `GET /{id}` – `POST` – `PUT /{id}` – `DELETE /{id}`
  * `GET /api/articulos/{id}/stock` – `POST /api/articulos/{id}/stock` *(upsert stock por tienda)*
  * **Catálogo por tienda**: `GET /api/tiendas/{tiendaId}/articulos`
* **Clientes**

  * `GET /api/clientes` – `GET /{id}` – `POST` – `PUT /{id}` – `DELETE /{id}`
  * **Historial**: `GET /api/clientes/{id}/compras` *(JWT; solo el propio cliente)*
* **Auth (JWT)**

  * `POST /api/auth/register`
  * `POST /api/auth/login`
* **Carrito** *(JWT)*

  * `POST /api/carrito/add`
  * `GET /api/carrito/items`
  * `POST /api/carrito/checkout`
  * `DELETE /api/carrito/clear`
  * `DELETE /api/carrito/item` *(body: `{ articuloId, tiendaId }`)*

---

## Configurar y correr el **Frontend (Angular)**

1. **Instalar dependencias**

```bash
cd spa/tienda-app
npm i
```

2. **Configurar API base**
   `src/environments/environment.ts`

```ts
export const environment = {
  apiBase: 'https://localhost:<PUERTO_API>/api'
};
```

3. **Angular Material + Animations**
   El proyecto usa Material; verifica que en `src/app/app.config.ts` estén:

```ts
import { provideAnimations } from '@angular/platform-browser/animations';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { authInterceptor } from './core/auth.interceptor';

export const appConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideAnimations()
  ]
};
```

4. **Arrancar**

```bash
ng serve
```

Abrir `http://localhost:4200`
Flujo: **Login → Tiendas → Catálogo → Carrito → Checkout**

---

## UI (Angular)

* **Standalone** (no `app.module.ts`), **Router**, **HttpClient**, **Auth interceptor** (Bearer), **Auth guard**.
* **Angular Material**: toolbar, cards, botones, snackbars.
* Páginas: **Login**, **Tiendas**, **Catálogo**, **Carrito**.

---

## Solución de problemas (rápido)

* **API 500 / Invalid object name**: la instancia no tiene las tablas → ejecutar scripts o verificar `ToTable` y conexión.
* **10061/40 (conexión SQL)**: host/puerto/instancia mal; habilita TCP/IP y usa `Server=127.0.0.1,<puerto>` o `Server=EQUIPO\INSTANCIA` con SQL Browser iniciado.
* **JWT clave corta (IDX10653)**: usa una clave ≥ 32 chars.
* **Swagger sin “Authorize”**: en `Program.cs` define el esquema Bearer en `AddSwaggerGen`.
* **Angular pantalla en blanco**: falta `<router-outlet>` o `templateUrl` mal; revisa consola (F12).
* **Material error `@angular/animations/browser`**: instala `@angular/animations` de la misma versión y añade `provideAnimations()`.

---

## Scripts SQL

* `01_schema.sql`: CREATE TABLE/VISTA
* `02_seed.sql`: datos demo (tienda, cliente, artículo, stock)

---

> **Nota rápida para ejecutar localmente**
> - Actualiza el puerto del API en `spa/tienda-app/src/environments/environment.ts` → `apiBase: 'https://localhost:<PUERTO_API>/api'`.
> - Configura la cadena de conexión en `api/Company.Project.Front/appsettings.json`: usa `Server=MI_PC\MI_INSTANCIA` **o** `Server=127.0.0.1,<PUERTO_SQL>`. (En dev puedes usar `TrustServerCertificate=True`).
> - JWT: usa una clave de **≥ 32 caracteres** en `Jwt:Key` (requerido para HS256).
> - Swagger: haz login (`/api/auth/login`) y en **Authorize** pega **solo** el token (sin `Bearer `).
> - CORS: la API debe permitir `http://localhost:4200` en `Program.cs` (`UseCors`/`AddCors`).
> - Si ves `Invalid object name ...`: ejecuta los scripts de `/db` en la **misma instancia** que usa la API **o** revisa los `ToTable(...)`.

---

## Licencia

MIT

---

## Cómo contribuir

PRs y sugerencias bienvenidas. Para cambios grandes, abre primero un issue.

