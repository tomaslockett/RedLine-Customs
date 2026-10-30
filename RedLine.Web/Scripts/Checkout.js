document.addEventListener('DOMContentLoaded', () => {
    const paymentOptions = document.querySelectorAll('.payment-option');
    const hfMetodoPago = document.getElementById('MainContent_hfMetodoPagoSeleccionado') || document.querySelector('input[name*="hfMetodoPagoSeleccionado"]');

    paymentOptions.forEach(option => {
        // Ignorar si está deshabilitado
        if (option.dataset.target === 'financing-panel') return;

        const header = option.querySelector('.option-header');
        header.addEventListener('click', () => {
            if (option.classList.contains('active')) return;

            paymentOptions.forEach(opt => {
                opt.classList.remove('active');
                const panel = document.getElementById(opt.dataset.target);
                if (panel) panel.classList.remove('open');
            });

            option.classList.add('active');
            const targetPanel = document.getElementById(option.dataset.target);
            if (targetPanel) targetPanel.classList.add('open');

            if (hfMetodoPago && option.dataset.metodo) {
                hfMetodoPago.value = option.dataset.metodo;
            }

            // Ocultar alerta de validación al cambiar de método
            const alerta = document.getElementById('jsAlertaCheckout');
            if (alerta) alerta.style.display = 'none';
        });
    });

    // Formateo de Número de Tarjeta (grupos de 4)
    const txtCardNumber = document.getElementById('txtCardNumber');
    if (txtCardNumber) {
        txtCardNumber.addEventListener('input', (e) => {
            let value = e.target.value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
            if (value.length > 16) value = value.substring(0, 16);

            let parts = [];
            for (let i = 0; i < value.length; i += 4) {
                parts.push(value.substring(i, i + 4));
            }
            e.target.value = parts.join(' ');
            limpiarErrorInput(e.target);
        });
    }

    // Formateo de Fecha de Expiración (MM/AA)
    const txtExpiry = document.getElementById('txtExpiry');
    if (txtExpiry) {
        txtExpiry.addEventListener('input', (e) => {
            let value = e.target.value.replace(/[^0-9]/gi, '');
            if (value.length > 4) value = value.substring(0, 4);

            if (value.length >= 2) {
                e.target.value = value.substring(0, 2) + '/' + value.substring(2, 4);
            } else {
                e.target.value = value;
            }
            limpiarErrorInput(e.target);
        });
    }

    // Formateo de CVV
    const txtCvv = document.getElementById('txtCvv');
    if (txtCvv) {
        txtCvv.addEventListener('input', (e) => {
            e.target.value = e.target.value.replace(/[^0-9]/gi, '').substring(0, 4);
            limpiarErrorInput(e.target);
        });
    }

    // Formateo de DNI Tarjeta
    const txtCardDni = document.getElementById('txtCardDni');
    if (txtCardDni) {
        txtCardDni.addEventListener('input', (e) => {
            e.target.value = e.target.value.replace(/[^0-9]/gi, '').substring(0, 8);
            limpiarErrorInput(e.target);
        });
    }

    // Formateo de CBU Transferencia (22 dígitos numéricos)
    const txtTransferCbu = document.getElementById('txtTransferCbu');
    if (txtTransferCbu) {
        txtTransferCbu.addEventListener('input', (e) => {
            e.target.value = e.target.value.replace(/[^0-9]/gi, '').substring(0, 22);
            limpiarErrorInput(e.target);
        });
    }

    // Formateo DNI Transferencia
    const txtTransferDni = document.getElementById('txtTransferDni');
    if (txtTransferDni) {
        txtTransferDni.addEventListener('input', (e) => {
            e.target.value = e.target.value.replace(/[^0-9]/gi, '').substring(0, 11);
            limpiarErrorInput(e.target);
        });
    }
});

function marcarErrorInput(element) {
    if (element) {
        element.classList.add('input-error');
    }
}

function limpiarErrorInput(element) {
    if (element) {
        element.classList.remove('input-error');
    }
}

function mostrarErrores(errores, primerElemento) {
    const alertDiv = document.getElementById('jsAlertaCheckout');
    const ulLista = document.getElementById('jsListaErrores');

    if (alertDiv && ulLista) {
        ulLista.innerHTML = '';
        errores.forEach(err => {
            const li = document.createElement('li');
            li.innerText = err;
            ulLista.appendChild(li);
        });
        alertDiv.style.display = 'block';
        alertDiv.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }

    if (primerElemento) {
        primerElemento.focus();
    }

    // Alerta nativa inmediata para advertir al usuario
    alert('Atención: Hay campos incompletos o con formato incorrecto:\n\n• ' + errores.join('\n• '));
}

// Validación estricta para Tarjeta de Crédito / Débito
function validarFormularioTarjeta() {
    const errores = [];
    let primerErrorElem = null;

    const txtCardName = document.getElementById('txtCardName');
    const txtCardDni = document.getElementById('txtCardDni');
    const txtCardNumber = document.getElementById('txtCardNumber');
    const txtExpiry = document.getElementById('txtExpiry');
    const txtCvv = document.getElementById('txtCvv');

    // 1. Nombre en la tarjeta
    const nombre = txtCardName ? txtCardName.value.trim() : '';
    if (!nombre || nombre.length < 3) {
        errores.push('Ingrese el nombre y apellido del titular tal como figura en la tarjeta (mínimo 3 caracteres).');
        marcarErrorInput(txtCardName);
        if (!primerErrorElem) primerErrorElem = txtCardName;
    } else {
        limpiarErrorInput(txtCardName);
    }

    // 2. DNI del titular
    const dni = txtCardDni ? txtCardDni.value.trim() : '';
    if (!dni || dni.length < 7 || !/^\d{7,8}$/.test(dni)) {
        errores.push('El DNI del titular debe tener entre 7 y 8 dígitos numéricos válidos.');
        marcarErrorInput(txtCardDni);
        if (!primerErrorElem) primerErrorElem = txtCardDni;
    } else {
        limpiarErrorInput(txtCardDni);
    }

    // 3. Número de tarjeta (16 dígitos)
    const numeroLimpio = txtCardNumber ? txtCardNumber.value.replace(/\s+/g, '') : '';
    if (!numeroLimpio || numeroLimpio.length !== 16 || !/^\d{16}$/.test(numeroLimpio)) {
        errores.push('El número de tarjeta debe contener exactamente 16 dígitos numéricos.');
        marcarErrorInput(txtCardNumber);
        if (!primerErrorElem) primerErrorElem = txtCardNumber;
    } else {
        limpiarErrorInput(txtCardNumber);
    }

    // 4. Expiración MM/AA
    const expiry = txtExpiry ? txtExpiry.value.trim() : '';
    let expiryValida = false;
    if (/^(0[1-9]|1[0-2])\/\d{2}$/.test(expiry)) {
        const partes = expiry.split('/');
        const mes = parseInt(partes[0], 10);
        const anio = parseInt('20' + partes[1], 10);
        const now = new Date();
        const curAnio = now.getFullYear();
        const curMes = now.getMonth() + 1;

        if (anio > curAnio || (anio === curAnio && mes >= curMes)) {
            expiryValida = true;
        }
    }

    if (!expiryValida) {
        errores.push('La fecha de vencimiento debe tener formato MM/AA válido (mes entre 01-12) y no estar vencida.');
        marcarErrorInput(txtExpiry);
        if (!primerErrorElem) primerErrorElem = txtExpiry;
    } else {
        limpiarErrorInput(txtExpiry);
    }

    // 5. CVV
    const cvv = txtCvv ? txtCvv.value.trim() : '';
    if (!cvv || (cvv.length !== 3 && cvv.length !== 4) || !/^\d{3,4}$/.test(cvv)) {
        errores.push('El código de seguridad (CVV) debe contener 3 o 4 dígitos numéricos.');
        marcarErrorInput(txtCvv);
        if (!primerErrorElem) primerErrorElem = txtCvv;
    } else {
        limpiarErrorInput(txtCvv);
    }

    if (errores.length > 0) {
        mostrarErrores(errores, primerErrorElem);
        return false;
    }

    const alertDiv = document.getElementById('jsAlertaCheckout');
    if (alertDiv) alertDiv.style.display = 'none';

    return true;
}

// Validación estricta para Transferencia Bancaria
function validarFormularioTransferencia() {
    const errores = [];
    let primerErrorElem = null;

    const txtTransferTitular = document.getElementById('txtTransferTitular');
    const txtTransferDni = document.getElementById('txtTransferDni');
    const txtTransferBanco = document.getElementById('txtTransferBanco');
    const txtTransferCbu = document.getElementById('txtTransferCbu');
    const txtTransferComprobante = document.getElementById('txtTransferComprobante');
    const txtTransferTelefono = document.getElementById('txtTransferTelefono');

    // 1. Titular
    const titular = txtTransferTitular ? txtTransferTitular.value.trim() : '';
    if (!titular || titular.length < 3) {
        errores.push('Debe ingresar el nombre del titular que realiza la transferencia (mínimo 3 letras).');
        marcarErrorInput(txtTransferTitular);
        if (!primerErrorElem) primerErrorElem = txtTransferTitular;
    } else {
        limpiarErrorInput(txtTransferTitular);
    }

    // 2. DNI / CUIT
    const dni = txtTransferDni ? txtTransferDni.value.trim() : '';
    if (!dni || dni.length < 7 || !/^\d{7,11}$/.test(dni)) {
        errores.push('El DNI o CUIT del titular emisor debe contener entre 7 y 11 dígitos numéricos.');
        marcarErrorInput(txtTransferDni);
        if (!primerErrorElem) primerErrorElem = txtTransferDni;
    } else {
        limpiarErrorInput(txtTransferDni);
    }

    // 3. Banco emisor
    const banco = txtTransferBanco ? txtTransferBanco.value.trim() : '';
    if (!banco || banco.length < 2) {
        errores.push('Indique el banco o billetera digital desde la cual transfirió.');
        marcarErrorInput(txtTransferBanco);
        if (!primerErrorElem) primerErrorElem = txtTransferBanco;
    } else {
        limpiarErrorInput(txtTransferBanco);
    }

    // 4. CBU / CVU (22 dígitos)
    const cbu = txtTransferCbu ? txtTransferCbu.value.trim() : '';
    if (!cbu || cbu.length !== 22 || !/^\d{22}$/.test(cbu)) {
        errores.push('El CBU o CVU de la cuenta de origen debe contener exactamente 22 dígitos numéricos.');
        marcarErrorInput(txtTransferCbu);
        if (!primerErrorElem) primerErrorElem = txtTransferCbu;
    } else {
        limpiarErrorInput(txtTransferCbu);
    }

    // 5. N° de Comprobante / Referencia
    const comprobante = txtTransferComprobante ? txtTransferComprobante.value.trim() : '';
    if (!comprobante || comprobante.length < 4) {
        errores.push('Ingrese el número de comprobante o referencia emitido por su entidad bancaria.');
        marcarErrorInput(txtTransferComprobante);
        if (!primerErrorElem) primerErrorElem = txtTransferComprobante;
    } else {
        limpiarErrorInput(txtTransferComprobante);
    }

    // 6. Teléfono
    const tel = txtTransferTelefono ? txtTransferTelefono.value.trim() : '';
    if (!tel || tel.length < 8 || !/^\d{8,15}$/.test(tel.replace(/[\s\-\+]/g, ''))) {
        errores.push('Ingrese un número de teléfono / WhatsApp válido para contacto y verificación.');
        marcarErrorInput(txtTransferTelefono);
        if (!primerErrorElem) primerErrorElem = txtTransferTelefono;
    } else {
        limpiarErrorInput(txtTransferTelefono);
    }

    if (errores.length > 0) {
        mostrarErrores(errores, primerErrorElem);
        return false;
    }

    const alertDiv = document.getElementById('jsAlertaCheckout');
    if (alertDiv) alertDiv.style.display = 'none';

    return true;
}
