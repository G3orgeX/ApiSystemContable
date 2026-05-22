# Integración de APIs de Tarjetas y Consumos

## Resumen

Se han creado los siguientes endpoints en el backend para gestionar tarjetas y consumos:

### Backend APIs Creadas

#### Tarjetas
- **GET** `/api/tarjetas` - Obtener todas las tarjetas del usuario autenticado
- **GET** `/api/tarjetas/{id}` - Obtener una tarjeta específica
- **POST** `/api/tarjetas` - Crear una nueva tarjeta
- **PUT** `/api/tarjetas/{id}` - Actualizar una tarjeta
- **DELETE** `/api/tarjetas/{id}` - Eliminar una tarjeta

#### Consumos
- **GET** `/api/consumos` - Obtener todos los consumos del usuario autenticado
- **GET** `/api/consumos/tarjeta/{idTarjeta}` - Obtener consumos de una tarjeta específica
- **GET** `/api/consumos/{id}` - Obtener un consumo específico
- **POST** `/api/consumos` - Crear un nuevo consumo
- **PUT** `/api/consumos/{id}` - Actualizar un consumo
- **DELETE** `/api/consumos/{id}` - Eliminar un consumo

### Seguridad
- Todos los endpoints requieren autenticación con JWT
- El usuario solo puede acceder a sus propias tarjetas y consumos
- El token se obtiene del header `Authorization: Bearer {token}`

## Cambios en el Backend

### Archivos Creados:
1. **DTOs**:
   - `DTOs/TarjetaDto.cs` - DTOs para tarjetas
   - `DTOs/ConsumoDto.cs` - DTOs para consumos

2. **Servicios**:
   - `Services/ITarjetaService.cs` - Interfaz de servicio
   - `Services/TarjetaService.cs` - Implementación
   - `Services/IConsumoService.cs` - Interfaz de servicio
   - `Services/ConsumoService.cs` - Implementación

3. **Controladores**:
   - `Controllers/TarjetasController.cs` - Endpoints de tarjetas
   - `Controllers/ConsumosController.cs` - Endpoints de consumos

### Archivos Modificados:
- `Models/Tarjeta.cs` - Agregado `IdUsuario`
- `Data/AppDbContext.cs` - Mapeo de `IdUsuario` en Tarjeta
- `Program.cs` - Registro de servicios

## Frontend Integration

### Paso 1: Copiar archivo de integración
Copia el archivo `tarjetas-api-integration.js` a tu proyecto frontend.

### Paso 2: Incluir en HTML
Agrega esta línea en tu `tarjetas.html` DESPUÉS del script `app.js`:

```html
<script src="path/to/tarjetas-api-integration.js"></script>
```

### Paso 3: Asegurar Token en localStorage
El script busca el token JWT en `localStorage.getItem('authToken')`. 
Asegúrate de que tu script de login guarde el token correctamente:

```javascript
// En tu app.js (después del login exitoso)
localStorage.setItem('authToken', response.token);
```

## Estructura de Datos

### Tarjeta
```json
{
  "idTarjeta": 1,
  "nombre": "Visa",
  "titular": "Juan Pérez",
  "tipo": "credito",
  "diaCierre": 15,
  "activo": true
}
```

### Consumo
```json
{
  "idConsumo": 1,
  "idTarjeta": 1,
  "fechaCompra": "2026-05-22T00:00:00",
  "concepto": "Compra en supermercado",
  "montoTotal": 150.50,
  "cuotas": 1,
  "valorCuota": 150.50,
  "esDebitoAutomatico": false
}
```

## Notas Importantes

1. **Base de Datos**: Asegúrate de ejecutar las migraciones necesarias para agregar `IdUsuario` a la tabla `tarjetas`.

2. **CORS**: El backend ya tiene CORS configurado para `http://localhost:5500` y `http://127.0.0.1:5500`.

3. **Autenticación**: Todos los endpoints requieren un JWT válido en el header `Authorization`.

4. **Formato de Fechas**: El frontend maneja fechas en formato ISO. El servidor las convierte automáticamente.

5. **Validación**: El backend valida que:
   - Las tarjetas pertenezcan al usuario autenticado
   - Los consumos estén asociados a tarjetas válidas
   - Los montos sean mayores a 0
