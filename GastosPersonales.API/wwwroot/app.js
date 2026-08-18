const API_URL = '/api';
let categoriesChartInstance = null;
// ================= ESTADO INICIAL Y NAVEGACIÓN =================
document.addEventListener('DOMContentLoaded', () => {
    const token = localStorage.getItem('token');
    if (token) {
        showApp();
    } else {
        showAuth();
    }
    // Poner fechas por defecto en el filtro de gastos (mes actual)
    const now = new Date();
    const firstDay = new Date(now.getFullYear(), now.getMonth(), 1).toISOString().split('T')[0];
    const lastDay = new Date(now.getFullYear(), now.getMonth() + 1, 0).toISOString().split('T')[0];

    if (document.getElementById('filter-inicio')) document.getElementById('filter-inicio').value = firstDay;
    if (document.getElementById('filter-fin')) document.getElementById('filter-fin').value = lastDay;
    if (document.getElementById('gasto-fecha')) document.getElementById('gasto-fecha').value = now.toISOString().split('T')[0];
    if (document.getElementById('pres-mes')) document.getElementById('pres-mes').value = now.getMonth() + 1;
    if (document.getElementById('pres-anio')) document.getElementById('pres-anio').value = now.getFullYear();
});
function showAuth() {
    document.getElementById('auth-section').classList.remove('hidden');
    document.getElementById('app-section').classList.add('hidden');
}
function showApp() {
    document.getElementById('auth-section').classList.add('hidden');
    document.getElementById('app-section').classList.remove('hidden');

    const userName = localStorage.getItem('userName') || 'Usuario';
    document.getElementById('user-display-name').innerText = userName;
    // Cargar datos iniciales
    loadCategoriasDropdowns();
    showView('dashboard');
}
function switchAuthTab(tab) {
    const loginForm = document.getElementById('login-form');
    const regForm = document.getElementById('register-form');
    const tabs = document.querySelectorAll('.tab-btn');
    document.getElementById('auth-error').classList.add('hidden');
    if (tab === 'login') {
        loginForm.classList.remove('hidden');
        regForm.classList.add('hidden');
        tabs[0].classList.add('active');
        tabs[1].classList.remove('active');
    } else {
        loginForm.classList.add('hidden');
        regForm.classList.remove('hidden');
        tabs[0].classList.remove('active');
        tabs[1].classList.add('active');
    }
}
function showView(viewId) {
    // Ocultar todas las vistas
    document.querySelectorAll('.view-panel').forEach(el => el.classList.add('hidden'));
    document.querySelectorAll('.nav-item').forEach(el => el.classList.remove('active'));
    // Mostrar vista seleccionada
    const target = document.getElementById(`view-${viewId}`);
    if (target) target.classList.remove('hidden');
    // Títulos
    const titles = {
        'dashboard': 'Dashboard General y Métricas',
        'gastos': 'Gestión de Gastos Personales',
        'presupuestos': 'Límites de Presupuesto Mensual',
        'categorias': 'Categorías de Gastos',
        'metodos': 'Métodos de Pago',
        'perfil': 'Mi Perfil de Usuario'
    };
    document.getElementById('view-title').innerText = titles[viewId] || 'Dashboard';
    // Cargar datos según la vista
    if (viewId === 'dashboard') loadDashboard();
    if (viewId === 'gastos') loadGastos();
    if (viewId === 'presupuestos') loadPresupuestos();
    if (viewId === 'categorias') loadCategorias();
    if (viewId === 'metodos') loadMetodos();
    if (viewId === 'perfil') loadProfile();
}
// ================= PETICIONES HTTP AUXILIARES =================
function getHeaders(isJson = true) {
    const headers = {};
    const token = localStorage.getItem('token');
    if (token) headers['Authorization'] = `Bearer ${token}`;
    if (isJson) headers['Content-Type'] = 'application/json';
    return headers;
}
// ================= AUTENTICACIÓN =================
async function handleLogin(e) {
    e.preventDefault();
    const email = document.getElementById('login-email').value;
    const password = document.getElementById('login-password').value;
    try {
        const res = await fetch(`${API_URL}/Auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        if (!res.ok) throw new Error('Credenciales incorrectas o usuario no encontrado.');
        const data = await res.json();
        localStorage.setItem('token', data.token);
        localStorage.setItem('userName', data.nombre);
        showToast('¡Bienvenido al sistema!', 'success');
        showApp();
    } catch (err) {
        showAuthError(err.message);
    }
}
async function handleRegister(e) {
    e.preventDefault();
    const nombre = document.getElementById('reg-name').value;
    const email = document.getElementById('reg-email').value;
    const password = document.getElementById('reg-password').value;
    try {
        const res = await fetch(`${API_URL}/Auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ nombre, email, password })
        });
        if (!res.ok) throw new Error('El correo ya se encuentra registrado.');
        const data = await res.json();
        localStorage.setItem('token', data.token);
        localStorage.setItem('userName', data.nombre);
        showToast('¡Cuenta creada con éxito!', 'success');
        showApp();
    } catch (err) {
        showAuthError(err.message);
    }
}
function handleLogout() {
    localStorage.clear();
    showToast('Sesión cerrada.');
    showAuth();
}
function showAuthError(msg) {
    const errBox = document.getElementById('auth-error');
    errBox.innerText = msg;
    errBox.classList.remove('hidden');
}
// ================= DASHBOARD & REPORTES =================
async function loadDashboard() {
    const now = new Date();
    const mes = now.getMonth() + 1;
    const anio = now.getFullYear();
    try {
        // 1. Cargar Reporte Mensual
        const res = await fetch(`${API_URL}/Reportes/mensual?mes=${mes}&anio=${anio}`, {
            headers: getHeaders()
        });
        if (res.ok) {
            const data = await res.json();
            document.getElementById('kpi-total-gastado').innerText = `$${data.totalGastado.toFixed(2)}`;
            document.getElementById('kpi-mes-anterior').innerText = `$${data.totalGastadoMesAnterior.toFixed(2)}`;

            const varEl = document.getElementById('kpi-variacion');
            varEl.innerText = `${data.diferenciaPorcentual >= 0 ? '+' : ''}${data.diferenciaPorcentual}%`;
            varEl.style.color = data.diferenciaPorcentual > 0 ? 'var(--danger)' : 'var(--success)';
            renderChart(data.desgloseCategorias);
        }
        // 2. Cargar Alertas de Presupuestos (50/80/100%)
        const resAlertas = await fetch(`${API_URL}/Reportes/alertas-presupuesto?mes=${mes}&anio=${anio}`, {
            headers: getHeaders()
        });
        if (resAlertas.ok) {
            const alertas = await resAlertas.json();
            renderBudgetAlerts(alertas);
        }
    } catch (err) {
        console.error('Error al cargar dashboard:', err);
    }
}
function renderChart(desglose) {
    const ctx = document.getElementById('categoriesChart');
    if (!ctx) return;
    if (categoriesChartInstance) {
        categoriesChartInstance.destroy();
    }
    if (!desglose || desglose.length === 0) {
        ctx.style.display = 'none';
        return;
    }
    ctx.style.display = 'block';
    const labels = desglose.map(d => d.categoriaNombre);
    const data = desglose.map(d => d.montoTotal);
    const colors = ['#38bdf8', '#818cf8', '#c084fc', '#f472b6', '#fb7185', '#34d399', '#fbbf24'];
    categoriesChartInstance = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                data: data,
                backgroundColor: colors,
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: { position: 'bottom', labels: { color: '#94a3b8' } }
            }
        }
    });
}
function renderBudgetAlerts(alertas) {
    const container = document.getElementById('budget-alerts-list');
    if (!alertas || alertas.length === 0) {
        container.innerHTML = '<p class="text-muted">No tienes presupuestos asignados para este mes.</p>';
        return;
    }
    container.innerHTML = alertas.map(a => {
        let alertClass = 'normal';
        if (a.alertaNivel === 'Advertencia') alertClass = 'advertencia';
        if (a.alertaNivel === 'Critico') alertClass = 'critico';
        if (a.alertaNivel === 'Excedido') alertClass = 'excedido';
        const barWidth = Math.min(a.porcentajeConsumido, 100);
        return `
            <div class="budget-item">
                <div class="budget-header">
                    <span>${a.categoriaNombre}</span>
                    <div>
                        <span>$${a.montoGastado.toFixed(2)} / $${a.montoLimite.toFixed(2)}</span>
                        <span class="badge badge-${alertClass}">${a.alertaNivel} (${a.porcentajeConsumido}%)</span>
                    </div>
                </div>
                <div class="progress-track">
                    <div class="progress-bar bar-${alertClass}" style="width: ${barWidth}%;"></div>
                </div>
            </div>
        `;
    }).join('');
}
// ================= GASTOS =================
async function loadGastos() {
    const inicio = document.getElementById('filter-inicio').value;
    const fin = document.getElementById('filter-fin').value;
    const catId = document.getElementById('filter-categoria').value;
    let url = `${API_URL}/Gastos?`;
    if (inicio) url += `fechaInicio=${inicio}&`;
    if (fin) url += `fechaFin=${fin}&`;
    if (catId) url += `categoriaId=${catId}&`;
    try {
        const res = await fetch(url, { headers: getHeaders() });
        const data = await res.json();
        const tbody = document.getElementById('gastos-table-body');
        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="6" class="text-center">No se encontraron gastos registrados.</td></tr>';
            return;
        }
        tbody.innerHTML = data.map(g => `
            <tr>
                <td>${new Date(g.fecha).toLocaleDateString()}</td>
                <td>${g.descripcion || 'Sin descripción'}</td>
                <td><span class="badge" style="background:#334155;">${g.nombreCategoria}</span></td>
                <td>${g.nombreMetodoPago}</td>
                <td style="font-weight:700; color:var(--primary);">$${g.monto.toFixed(2)}</td>
                <td>
                    <button class="btn btn-outline-danger" style="padding:4px 8px; font-size:12px;" onclick="deleteGasto(${g.id})">
                        <i class="fa-solid fa-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        console.error(err);
    }
}
async function handleCreateGasto(e) {
    e.preventDefault();
    const monto = parseFloat(document.getElementById('gasto-monto').value);
    const fecha = document.getElementById('gasto-fecha').value;
    const categoriaId = parseInt(document.getElementById('gasto-categoria').value);
    const metodoPagoId = parseInt(document.getElementById('gasto-metodo').value);
    const descripcion = document.getElementById('gasto-desc').value;
    try {
        const res = await fetch(`${API_URL}/Gastos`, {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify({ monto, fecha, categoriaId, metodoPagoId, descripcion })
        });
        if (!res.ok) throw new Error('Error al registrar gasto');
        const result = await res.json();
        closeModal('modal-gasto');

        if (result.limitePresupuestoSuperado) {
            showToast('⚠️ ¡ATENCIÓN! Este gasto ha superado el presupuesto mensual establecido para esta categoría.', 'warning');
        } else {
            showToast('Gasto registrado con éxito.', 'success');
        }
        loadGastos();
    } catch (err) {
        showToast(err.message, 'danger');
    }
}
async function deleteGasto(id) {
    if (!confirm('¿Seguro que deseas eliminar este gasto?')) return;
    try {
        const res = await fetch(`${API_URL}/Gastos/${id}`, {
            method: 'DELETE',
            headers: getHeaders()
        });
        if (res.ok) {
            showToast('Gasto eliminado.');
            loadGastos();
        }
    } catch (err) {
        console.error(err);
    }
}
// ================= PRESUPUESTOS =================
async function loadPresupuestos() {
    try {
        const res = await fetch(`${API_URL}/Presupuestos`, { headers: getHeaders() });
        const data = await res.json();
        const tbody = document.getElementById('presupuestos-table-body');
        if (!data || data.length === 0) {
            tbody.innerHTML = '<tr><td colspan="4" class="text-center">No hay presupuestos asignados.</td></tr>';
            return;
        }
        tbody.innerHTML = data.map(p => `
            <tr>
                <td><strong>${p.nombreCategoria}</strong></td>
                <td>${p.mes} / ${p.anio}</td>
                <td style="font-weight:700; color:var(--success);">$${p.montoLimite.toFixed(2)}</td>
                <td>
                    <button class="btn btn-outline-danger" style="padding:4px 8px; font-size:12px;" onclick="deletePresupuesto(${p.id})">
                        <i class="fa-solid fa-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        console.error(err);
    }
}
async function handleCreatePresupuesto(e) {
    e.preventDefault();
    const categoriaId = parseInt(document.getElementById('pres-categoria').value);
    const montoLimite = parseFloat(document.getElementById('pres-monto').value);
    const mes = parseInt(document.getElementById('pres-mes').value);
    const anio = parseInt(document.getElementById('pres-anio').value);
    try {
        const res = await fetch(`${API_URL}/Presupuestos`, {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify({ categoriaId, montoLimite, mes, anio })
        });
        if (!res.ok) throw new Error('Ya existe un presupuesto para esta categoría en este mes/año.');
        closeModal('modal-presupuesto');
        showToast('Presupuesto asignado con éxito.', 'success');
        loadPresupuestos();
    } catch (err) {
        showToast(err.message, 'danger');
    }
}
async function deletePresupuesto(id) {
    if (!confirm('¿Eliminar este presupuesto?')) return;
    try {
        const res = await fetch(`${API_URL}/Presupuestos/${id}`, { method: 'DELETE', headers: getHeaders() });
        if (res.ok) {
            showToast('Presupuesto eliminado.');
            loadPresupuestos();
        }
    } catch (err) {
        console.error(err);
    }
}
// ================= CATEGORÍAS & MÉTODOS =================
async function loadCategorias() {
    try {
        const res = await fetch(`${API_URL}/Categorias`, { headers: getHeaders() });
        const data = await res.json();
        const tbody = document.getElementById('categorias-table-body');
        tbody.innerHTML = data.map(c => `
            <tr>
                <td><strong>${c.nombre}</strong></td>
                <td>${c.descripcion || 'Sin descripción'}</td>
                <td><span class="badge ${c.esActivo ? 'badge-normal' : 'badge-excedido'}">${c.esActivo ? 'Activo' : 'Inactivo'}</span></td>
                <td>
                    <button class="btn btn-outline-danger" style="padding:4px 8px; font-size:12px;" onclick="deleteCategoria(${c.id})">
                        <i class="fa-solid fa-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        console.error(err);
    }
}
async function handleCreateCategoria(e) {
    e.preventDefault();
    const nombre = document.getElementById('cat-nombre').value;
    const descripcion = document.getElementById('cat-desc').value;
    try {
        const res = await fetch(`${API_URL}/Categorias`, {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify({ nombre, descripcion })
        });
        if (!res.ok) throw new Error('Error al crear categoría.');
        closeModal('modal-categoria');
        showToast('Categoría creada.');
        loadCategorias();
        loadCategoriasDropdowns();
    } catch (err) {
        showToast(err.message, 'danger');
    }
}
async function deleteCategoria(id) {
    if (!confirm('¿Eliminar categoría? Si tiene gastos asociados no se podrá eliminar.')) return;
    try {
        const res = await fetch(`${API_URL}/Categorias/${id}`, { method: 'DELETE', headers: getHeaders() });
        if (res.ok) {
            showToast('Categoría eliminada.');
            loadCategorias();
            loadCategoriasDropdowns();
        } else {
            showToast('No se puede eliminar porque tiene gastos asociados.', 'danger');
        }
    } catch (err) {
        console.error(err);
    }
}
async function loadMetodos() {
    try {
        const res = await fetch(`${API_URL}/MetodosPago`, { headers: getHeaders() });
        const data = await res.json();
        const tbody = document.getElementById('metodos-table-body');
        tbody.innerHTML = data.map(m => `
            <tr>
                <td><strong>${m.nombre}</strong></td>
                <td><i class="fa-solid fa-credit-card"></i> ${m.icono || 'default'}</td>
                <td><span class="badge ${m.esActivo ? 'badge-normal' : 'badge-excedido'}">${m.esActivo ? 'Activo' : 'Inactivo'}</span></td>
                <td>
                    <button class="btn btn-outline-danger" style="padding:4px 8px; font-size:12px;" onclick="deleteMetodo(${m.id})">
                        <i class="fa-solid fa-trash"></i>
                    </button>
                </td>
            </tr>
        `).join('');
    } catch (err) {
        console.error(err);
    }
}
async function handleCreateMetodo(e) {
    e.preventDefault();
    const nombre = document.getElementById('met-nombre').value;
    const icono = document.getElementById('met-icono').value;
    try {
        const res = await fetch(`${API_URL}/MetodosPago`, {
            method: 'POST',
            headers: getHeaders(),
            body: JSON.stringify({ nombre, icono })
        });
        if (!res.ok) throw new Error('Error al crear método.');
        closeModal('modal-metodo');
        showToast('Método de pago creado.');
        loadMetodos();
        loadCategoriasDropdowns();
    } catch (err) {
        showToast(err.message, 'danger');
    }
}
async function deleteMetodo(id) {
    if (!confirm('¿Eliminar método de pago?')) return;
    try {
        const res = await fetch(`${API_URL}/MetodosPago/${id}`, { method: 'DELETE', headers: getHeaders() });
        if (res.ok) {
            showToast('Método eliminado.');
            loadMetodos();
            loadCategoriasDropdowns();
        }
    } catch (err) {
        console.error(err);
    }
}
async function loadCategoriasDropdowns() {
    try {
        const [catsRes, metsRes] = await Promise.all([
            fetch(`${API_URL}/Categorias`, { headers: getHeaders() }),
            fetch(`${API_URL}/MetodosPago`, { headers: getHeaders() })
        ]);
        const cats = await catsRes.json();
        const mets = await metsRes.json();
        // Llenar selects
        const fillSelect = (selectId, items, placeholder) => {
            const el = document.getElementById(selectId);
            if (!el) return;
            el.innerHTML = placeholder ? `<option value="">${placeholder}</option>` : '';
            items.forEach(i => {
                el.innerHTML += `<option value="${i.id}">${i.nombre}</option>`;
            });
        };
        fillSelect('filter-categoria', cats, 'Todas las categorías');
        fillSelect('gasto-categoria', cats);
        fillSelect('pres-categoria', cats);
        fillSelect('gasto-metodo', mets);
    } catch (err) {
        console.error(err);
    }
}
// ================= PERFIL =================
async function loadProfile() {
    try {
        const res = await fetch(`${API_URL}/Auth/profile`, { headers: getHeaders() });
        const data = await res.json();
        document.getElementById('perfil-nombre').value = data.nombre;
        document.getElementById('perfil-email').value = data.email;
    } catch (err) {
        console.error(err);
    }
}
async function handleUpdateProfile(e) {
    e.preventDefault();
    const nombre = document.getElementById('perfil-nombre').value;
    const newPassword = document.getElementById('perfil-password').value;
    try {
        const res = await fetch(`${API_URL}/Auth/profile`, {
            method: 'PUT',
            headers: getHeaders(),
            body: JSON.stringify({ nombre, newPassword: newPassword || null })
        });
        if (res.ok) {
            localStorage.setItem('userName', nombre);
            document.getElementById('user-display-name').innerText = nombre;
            showToast('Perfil actualizado correctamente.', 'success');
        }
    } catch (err) {
        showToast('Error al actualizar perfil.', 'danger');
    }
}
// ================= IMPORTACIÓN & EXPORTACIÓN =================
async function handleImportCsv(e) {
    const file = e.target.files[0];
    if (!file) return;
    const formData = new FormData();
    formData.append('archivo', file);
    try {
        showToast('Subiendo y procesando archivo CSV...');
        const token = localStorage.getItem('token');
        const res = await fetch(`${API_URL}/Gastos/importar-excel`, {
            method: 'POST',
            headers: { 'Authorization': `Bearer ${token}` },
            body: formData
        });
        if (!res.ok) throw new Error('Error al importar archivo CSV.');
        const data = await res.json();
        showToast(`¡Éxito! Se importaron ${data.length} gastos correctamente.`, 'success');
        loadGastos();
    } catch (err) {
        showToast(err.message, 'danger');
    } finally {
        e.target.value = '';
    }
}
function exportarReporte(formato) {
    const now = new Date();
    const mes = now.getMonth() + 1;
    const anio = now.getFullYear();
    const token = localStorage.getItem('token');
    showToast(`Generando reporte en formato ${formato.toUpperCase()}...`);
    fetch(`${API_URL}/Reportes/exportar/${formato}?mes=${mes}&anio=${anio}`, {
        headers: { 'Authorization': `Bearer ${token}` }
    })
        .then(response => {
            if (!response.ok) throw new Error('Error al exportar reporte.');
            return response.blob();
        })
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `ReporteGastos_${mes}_${anio}.${formato}`;
            document.body.appendChild(a);
            a.click();
            a.remove();
            showToast(`Reporte ${formato.toUpperCase()} descargado exitosamente.`, 'success');
        })
        .catch(err => {
            showToast(err.message, 'danger');
        });
}
// ================= MODALES & NOTIFICACIONES =================
function openModal(id) {
    document.getElementById(id).classList.remove('hidden');
}
function closeModal(id) {
    document.getElementById(id).classList.add('hidden');
}
function showToast(msg, type = 'info') {
    const toast = document.getElementById('toast');
    toast.innerText = msg;
    toast.style.borderColor = type === 'warning' ? 'var(--warning)' : (type === 'danger' ? 'var(--danger)' : 'var(--primary)');
    toast.classList.remove('hidden');
    setTimeout(() => toast.classList.add('hidden'), 4000);
}