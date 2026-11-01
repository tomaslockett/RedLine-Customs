document.addEventListener('DOMContentLoaded', () => {
    // Diccionario de textos dinámicos con respaldo local
    const i18n = Object.assign({
        tarjetaValida: 'Tarjeta válida',
        tarjetaInvalida: 'Número de tarjeta inválido',
        calculando: 'Calculando cotización...',
        errorServicio: 'Servicio de financiación no disponible momentáneamente',
        cuotasDe: 'cuotas fijas de'
    }, window.CheckoutI18n || {});

    // Referencias a elementos del DOM
    const paymentOptions = document.querySelectorAll('.payment-option');
    const hfMetodoPago = document.getElementById('hfMetodoPago');
    const hfTotalUsd = document.getElementById('hfTotalUsd');
    const hfPlanFinanciacionSeleccionado = document.getElementById('hfPlanFinanciacionSeleccionado');

    // Elementos del Conversor
    const ddlMonedaReferencia = document.getElementById('ddlMonedaReferencia');
    const lblResultadoConversion = document.getElementById('lblResultadoConversion');
    const lblDetalleTipoCambio = document.getElementById('lblDetalleTipoCambio');

    // Elementos de Tarjeta
    const txtCardNumber = document.getElementById('txtCardNumber');
    const txtExpiry = document.getElementById('txtExpiry');
    const txtCvv = document.getElementById('txtCvv');
    const brandVisa = document.getElementById('brandVisa');
    const brandMastercard = document.getElementById('brandMastercard');
    const brandAmex = document.getElementById('brandAmex');
    const cardValidationStatus = document.getElementById('cardValidationStatus');

    // Elementos de Transferencia Bancaria
    const lblMontoSugeridoValor = document.getElementById('lblMontoSugeridoValor');

    // Elementos del Simulador de Financiación
    const ddlCuotasFinanciacion = document.getElementById('ddlCuotasFinanciacion');
    const ddlAnticipoFinanciacion = document.getElementById('ddlAnticipoFinanciacion');
    const lblFinAnticipoVal = document.getElementById('lblFinAnticipoVal');
    const lblFinCapitalVal = document.getElementById('lblFinCapitalVal');
    const lblFinCuotaVal = document.getElementById('lblFinCuotaVal');
    const lblFinTasasVal = document.getElementById('lblFinTasasVal');
    const lblFinTotalPlanVal = document.getElementById('lblFinTotalPlanVal');

    // Invocador AJAX a FinanciacionService.asmx
    async function callFinanciacionService(methodName, payload) {
        const response = await fetch(`FinanciacionService.asmx/${methodName}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json; charset=utf-8'
            },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            throw new Error(`HTTP error ${response.status}`);
        }

        const data = await response.json();
        return data.d; // ASP.NET ASMX envuelve la respuesta JSON en la propiedad "d"
    }

    function getTotalUsd() {
        if (!hfTotalUsd || !hfTotalUsd.value) return 0;
        const parsed = parseFloat(hfTotalUsd.value);
        return isNaN(parsed) ? 0 : parsed;
    }

    // ---------------------------------------------------------------------
    // 1. Selector de Pestañas de Métodos de Pago
    // ---------------------------------------------------------------------
    paymentOptions.forEach(option => {
        const header = option.querySelector('.option-header');
        if (!header) return;

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

            // Actualización de método de pago según pestaña
            if (hfMetodoPago) {
                if (option.dataset.target === 'bank-panel') {
                    hfMetodoPago.value = 'Transferencia Bancaria';
                    actualizarTransferenciaBancaria();
                } else if (option.dataset.target === 'financing-panel') {
                    const cuotas = ddlCuotasFinanciacion ? ddlCuotasFinanciacion.value : '24';
                    hfMetodoPago.value = `Financiación (${cuotas} cuotas)`;
                    actualizarFinanciacion();
                } else {
                    // CONTRATO INTOCABLE: TextosComercialesVentas.MetodoPagoTarjeta
                    hfMetodoPago.value = 'Tarjeta de Crédito';
                }
            }
        });
    });

    // ---------------------------------------------------------------------
    // 2. Conversor de Divisas de Referencia
    // ---------------------------------------------------------------------
    async function actualizarCotizacion() {
        if (!ddlMonedaReferencia || !lblResultadoConversion) return;
        const total = getTotalUsd();
        if (total <= 0) return;

        const moneda = ddlMonedaReferencia.value;

        try {
            lblResultadoConversion.textContent = i18n.calculando;
            const res = await callFinanciacionService('ConvertirMoneda', {
                montoUsd: total,
                monedaDestino: moneda
            });

            if (res) {
                lblResultadoConversion.textContent = res.MontoFormateado;
                if (lblDetalleTipoCambio) {
                    if (res.CodigoMoneda === 'USD') {
                        lblDetalleTipoCambio.textContent = '1 USD = 1.00 USD';
                    } else {
                        lblDetalleTipoCambio.textContent = `1 USD = ${res.Simbolo} ${res.TasaCambio.toLocaleString('es-AR')}`;
                    }
                }
            }
        } catch (err) {
            console.error('Error al convertir moneda:', err);
            lblResultadoConversion.textContent = `$${total.toLocaleString('en-US')} USD`;
        }
    }

    if (ddlMonedaReferencia) {
        ddlMonedaReferencia.addEventListener('change', actualizarCotizacion);
        actualizarCotizacion();
    }

    // ---------------------------------------------------------------------
    // 3. Transferencia Bancaria (Cálculo sugerido en ARS MEP)
    // ---------------------------------------------------------------------
    async function actualizarTransferenciaBancaria() {
        if (!lblMontoSugeridoValor) return;
        const total = getTotalUsd();
        if (total <= 0) return;

        try {
            const res = await callFinanciacionService('ConvertirMoneda', {
                montoUsd: total,
                monedaDestino: 'ARS_MEP'
            });

            if (res) {
                lblMontoSugeridoValor.textContent = `${res.MontoFormateado} (TC MEP: $${res.TasaCambio.toLocaleString('es-AR')})`;
            }
        } catch (err) {
            console.error('Error al consultar equivalente bancario:', err);
        }
    }

    // ---------------------------------------------------------------------
    // 4. Simulador Interactivo de Financiación (Sistema Francés)
    // ---------------------------------------------------------------------
    async function actualizarFinanciacion() {
        const total = getTotalUsd();
        if (total <= 0) return;

        const cuotas = ddlCuotasFinanciacion ? parseInt(ddlCuotasFinanciacion.value, 10) : 24;
        const anticipo = ddlAnticipoFinanciacion ? parseFloat(ddlAnticipoFinanciacion.value) : 30;

        try {
            const plan = await callFinanciacionService('CalcularPlanFinanciacion', {
                montoUsd: total,
                cuotas: cuotas,
                porcentajeAnticipo: anticipo
            });

            if (plan) {
                if (lblFinAnticipoVal) lblFinAnticipoVal.textContent = `$${plan.AnticipoUsd.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} USD`;
                if (lblFinCapitalVal) lblFinCapitalVal.textContent = `$${plan.CapitalFinanciadoUsd.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} USD`;
                if (lblFinCuotaVal) lblFinCuotaVal.textContent = `$${plan.ValorCuotaUsd.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} USD`;
                if (lblFinTasasVal) lblFinTasasVal.textContent = `${plan.TnaPorcentaje}% / ${plan.CftPorcentaje}%`;
                if (lblFinTotalPlanVal) lblFinTotalPlanVal.textContent = `$${plan.TotalPlanUsd.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 })} USD`;

                // Actualizar HiddenField si la pestaña activa es Financiación
                const activeOption = document.querySelector('.payment-option.active');
                if (activeOption && activeOption.dataset.target === 'financing-panel') {
                    if (hfMetodoPago) {
                        hfMetodoPago.value = `Financiación (${cuotas} cuotas)`;
                    }
                }
                if (hfPlanFinanciacionSeleccionado) {
                    hfPlanFinanciacionSeleccionado.value = plan.ResumenPlan;
                }
            }
        } catch (err) {
            console.error('Error al simular financiación:', err);
        }
    }

    if (ddlCuotasFinanciacion) ddlCuotasFinanciacion.addEventListener('change', actualizarFinanciacion);
    if (ddlAnticipoFinanciacion) ddlAnticipoFinanciacion.addEventListener('change', actualizarFinanciacion);

    // ---------------------------------------------------------------------
    // 5. Validación de Tarjeta de Crédito (Luhn en vivo + Detección de Marca)
    // ---------------------------------------------------------------------
    let debounceLuhnTimer = null;

    if (txtCardNumber) {
        txtCardNumber.addEventListener('input', (e) => {
            let value = e.target.value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
            let matches = value.match(/\d{4,19}/g);
            let match = (matches && matches[0]) || '';
            let parts = [];

            for (let i = 0, len = match.length; i < len; i += 4) {
                parts.push(match.substring(i, i + 4));
            }

            if (parts.length > 0) {
                e.target.value = parts.join(' ');
            } else {
                e.target.value = value;
            }

            clearTimeout(debounceLuhnTimer);
            debounceLuhnTimer = setTimeout(async () => {
                const cleanNumber = e.target.value.replace(/\s+/g, '');

                // Reset de marcas
                if (brandVisa) brandVisa.classList.remove('brand-active');
                if (brandMastercard) brandMastercard.classList.remove('brand-active');
                if (brandAmex) brandAmex.classList.remove('brand-active');

                if (cleanNumber.length < 4) {
                    if (cardValidationStatus) {
                        cardValidationStatus.innerHTML = '';
                        cardValidationStatus.className = 'card-validation-status';
                    }
                    return;
                }

                try {
                    const validacion = await callFinanciacionService('ValidarTarjeta', {
                        numeroTarjeta: cleanNumber
                    });

                    if (validacion) {
                        // Iluminar marca detectada
                        if (validacion.Marca === 'Visa' && brandVisa) brandVisa.classList.add('brand-active');
                        if (validacion.Marca === 'Mastercard' && brandMastercard) brandMastercard.classList.add('brand-active');
                        if (validacion.Marca === 'Amex' && brandAmex) brandAmex.classList.add('brand-active');

                        if (cardValidationStatus && cleanNumber.length >= 13) {
                            if (validacion.EsValida) {
                                cardValidationStatus.className = 'card-validation-status valid';
                                cardValidationStatus.innerHTML = `<i class="fa-solid fa-circle-check"></i> ${i18n.tarjetaValida} (${validacion.Marca})`;
                            } else {
                                cardValidationStatus.className = 'card-validation-status invalid';
                                cardValidationStatus.innerHTML = `<i class="fa-solid fa-triangle-exclamation"></i> ${i18n.tarjetaInvalida}`;
                            }
                        } else if (cardValidationStatus) {
                            cardValidationStatus.innerHTML = '';
                            cardValidationStatus.className = 'card-validation-status';
                        }
                    }
                } catch (err) {
                    console.error('Error al validar tarjeta por Luhn:', err);
                }
            }, 300);
        });
    }

    if (txtExpiry) {
        txtExpiry.addEventListener('input', (e) => {
            let value = e.target.value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
            if (value.length >= 2) {
                e.target.value = value.substring(0, 2) + '/' + value.substring(2, 4);
            } else {
                e.target.value = value;
            }
        });
    }

    if (txtCvv) {
        txtCvv.addEventListener('input', (e) => {
            e.target.value = e.target.value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
        });
    }
});
