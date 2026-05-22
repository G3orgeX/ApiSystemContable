// API Configuration
const API_BASE_URL = 'https://localhost:7154/api';

// Get JWT token from localStorage
function getAuthToken() {
    return localStorage.getItem('authToken');
}

// Set auth header
function getAuthHeaders() {
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${getAuthToken()}`
    };
}

// Handle API errors
function handleApiError(error, defaultMessage = 'Error en la solicitud') {
    console.error('API Error:', error);
    alert(defaultMessage);
}

// ============ TARJETAS API ============

async function loadTarjetasFromAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/tarjetas`, {
            method: 'GET',
            headers: getAuthHeaders()
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const result = await response.json();
        return result.data || [];
    } catch (error) {
        handleApiError(error, 'Error al cargar tarjetas');
        return [];
    }
}

async function createTarjetaAPI(tarjetaData) {
    try {
        const response = await fetch(`${API_BASE_URL}/tarjetas`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(tarjetaData)
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const result = await response.json();
        return result;
    } catch (error) {
        handleApiError(error, 'Error al crear tarjeta');
        throw error;
    }
}

async function updateTarjetaAPI(idTarjeta, tarjetaData) {
    try {
        const response = await fetch(`${API_BASE_URL}/tarjetas/${idTarjeta}`, {
            method: 'PUT',
            headers: getAuthHeaders(),
            body: JSON.stringify(tarjetaData)
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const result = await response.json();
        return result.data;
    } catch (error) {
        handleApiError(error, 'Error al actualizar tarjeta');
        throw error;
    }
}

async function deleteTarjetaAPI(idTarjeta) {
    try {
        const response = await fetch(`${API_BASE_URL}/tarjetas/${idTarjeta}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        return true;
    } catch (error) {
        handleApiError(error, 'Error al eliminar tarjeta');
        throw error;
    }
}

// ============ CONSUMOS API ============

async function loadConsumosFromAPI() {
    try {
        const response = await fetch(`${API_BASE_URL}/consumos`, {
            method: 'GET',
            headers: getAuthHeaders()
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const result = await response.json();
        return result.data || [];
    } catch (error) {
        handleApiError(error, 'Error al cargar consumos');
        return [];
    }
}

async function loadConsumosByTarjetaAPI(idTarjeta) {
    try {
        const response = await fetch(`${API_BASE_URL}/consumos/tarjeta/${idTarjeta}`, {
            method: 'GET',
            headers: getAuthHeaders()
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const result = await response.json();
        return result.data || [];
    } catch (error) {
        handleApiError(error, 'Error al cargar consumos de la tarjeta');
        return [];
    }
}

async function createConsumoAPI(consumoData) {
    try {
        const response = await fetch(`${API_BASE_URL}/consumos`, {
            method: 'POST',
            headers: getAuthHeaders(),
            body: JSON.stringify(consumoData)
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const result = await response.json();
        return result;
    } catch (error) {
        handleApiError(error, 'Error al crear consumo');
        throw error;
    }
}

async function updateConsumoAPI(idConsumo, consumoData) {
    try {
        const response = await fetch(`${API_BASE_URL}/consumos/${idConsumo}`, {
            method: 'PUT',
            headers: getAuthHeaders(),
            body: JSON.stringify(consumoData)
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const result = await response.json();
        return result.data;
    } catch (error) {
        handleApiError(error, 'Error al actualizar consumo');
        throw error;
    }
}

async function deleteConsumoAPI(idConsumo) {
    try {
        const response = await fetch(`${API_BASE_URL}/consumos/${idConsumo}`, {
            method: 'DELETE',
            headers: getAuthHeaders()
        });

        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        return true;
    } catch (error) {
        handleApiError(error, 'Error al eliminar consumo');
        throw error;
    }
}

// ============ TARJETAS PAGE INTEGRATION ============

document.addEventListener('DOMContentLoaded', async () => {
    const masterCardForm = document.getElementById('masterCardForm');
    const masterCardsBody = document.getElementById('masterCardsBody');
    const cardMovementForm = document.getElementById('cardMovementForm');
    const cardTableBody = document.getElementById('cardTableBody');
    const tarjetaSelect = document.getElementById('tarjeta_id');

    // Default dates
    const dateCompraInput = document.getElementById('fecha_consumo');
    if (dateCompraInput) dateCompraInput.valueAsDate = new Date();

    // Load cards from API
    const loadCards = async () => {
        if (!masterCardsBody) return;

        const tarjetas = await loadTarjetasFromAPI();

        // Populate Master Cards Table
        masterCardsBody.innerHTML = '';

        // Populate Select Dropdown
        if (tarjetaSelect) {
            tarjetaSelect.innerHTML = '<option value="" disabled selected>Elige una tarjeta...</option>';
        }

        tarjetas.forEach(tarjeta => {
            // Row
            const row = document.createElement('tr');
            row.innerHTML = `
                <td><strong>${tarjeta.nombre}</strong></td>
                <td>${tarjeta.titular}</td>
                <td><span class="badge ${tarjeta.tipo === 'credito' ? 'expense' : 'income'}">${tarjeta.tipo === 'credito' ? 'Crédito' : 'Débito'}</span></td>
                <td>Día ${tarjeta.diaCierre}</td>
            `;
            masterCardsBody.appendChild(row);

            // Select Option
            if (tarjetaSelect) {
                const option = document.createElement('option');
                option.value = tarjeta.idTarjeta;
                option.textContent = `${tarjeta.nombre} (${tarjeta.titular})`;
                tarjetaSelect.appendChild(option);
            }
        });
    };

    // Load card movements from API
    const loadCardMovements = async () => {
        if (!cardTableBody) return;

        const consumos = await loadConsumosFromAPI();
        const tarjetas = await loadTarjetasFromAPI();

        cardTableBody.innerHTML = '';

        consumos.forEach((mov) => {
            const tarjeta = tarjetas.find(t => t.idTarjeta == mov.idTarjeta);
            if (!tarjeta) return;

            const row = document.createElement('tr');

            const formattedMonto = new Intl.NumberFormat('en-US', {
                style: 'currency',
                currency: 'USD'
            }).format(mov.montoTotal);

            let badgeClass = tarjeta.tipo === 'credito' ? 'expense' : 'income';
            let badgeText = tarjeta.tipo === 'credito' ? 'Crédito' : 'Débito';

            row.innerHTML = `
                <td>${new Date(mov.fechaCompra).toLocaleDateString('es-AR')}</td>
                <td>
                    <strong>${tarjeta.nombre}</strong> <br>
                    <small style="color: var(--text-secondary)">${tarjeta.titular}</small> <br>
                    <span class="badge ${badgeClass}" style="font-size: 0.6rem;">${badgeText}</span>
                </td>
                <td>${mov.concepto}</td>
                <td style="color: var(--danger-color)">-${formattedMonto}</td>
            `;
            cardTableBody.appendChild(row);
        });
    };

    // Initialize Tarjetas Page
    await loadCards();
    await loadCardMovements();

    // Handle New Card Registration
    if (masterCardForm) {
        masterCardForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const tipo = document.getElementById('tipo_tarjeta').value;
            const nombre = document.getElementById('nombre_tarjeta').value;
            const titular = document.getElementById('propietario_tarjeta').value;
            const diaCierre = parseInt(document.getElementById('dia_cierre').value);

            try {
                const newTarjeta = {
                    nombre,
                    titular,
                    tipo,
                    diaCierre,
                    activo: true
                };

                await createTarjetaAPI(newTarjeta);
                alert('Tarjeta registrada exitosamente');
                await loadCards();
                masterCardForm.reset();
            } catch (error) {
                console.error('Error:', error);
            }
        });
    }

    // Handle New Consumption Registration
    if (cardMovementForm) {
        cardMovementForm.addEventListener('submit', async (e) => {
            e.preventDefault();

            const idTarjeta = document.getElementById('tarjeta_id').value;
            if (!idTarjeta) {
                alert("Por favor selecciona una tarjeta primero.");
                return;
            }

            const fechaCompra = document.getElementById('fecha_consumo').value;
            const cuotas = parseInt(document.getElementById('cuotas').value) || 1;
            const concepto = document.getElementById('concepto_tarjeta').value;
            const montoTotal = parseFloat(document.getElementById('monto_tarjeta').value);

            if (isNaN(montoTotal) || montoTotal <= 0) {
                alert("Por favor ingresa un monto válido mayor a 0.");
                return;
            }

            try {
                const newConsumo = {
                    idTarjeta: parseInt(idTarjeta),
                    fechaCompra,
                    concepto,
                    montoTotal,
                    cuotas,
                    esDebitoAutomatico: false
                };

                await createConsumoAPI(newConsumo);
                alert('Consumo registrado exitosamente');
                await loadCardMovements();

                // Reset only consumption fields
                document.getElementById('concepto_tarjeta').value = '';
                document.getElementById('monto_tarjeta').value = '';
                document.getElementById('cuotas').value = '1';
                document.getElementById('concepto_tarjeta').focus();
            } catch (error) {
                console.error('Error:', error);
            }
        });
    }
});
