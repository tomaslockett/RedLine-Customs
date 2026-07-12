document.addEventListener('DOMContentLoaded', () => {
    const paymentOptions = document.querySelectorAll('.payment-option');
    
    paymentOptions.forEach(option => {
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
        });
    });

    const txtCardNumber = document.getElementById('txtCardNumber');
    if (txtCardNumber) {
        txtCardNumber.addEventListener('input', (e) => {
            let value = e.target.value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
            let matches = value.match(/\d{4,16}/g);
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
        });
    }

    const txtExpiry = document.getElementById('txtExpiry');
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

    const txtCvv = document.getElementById('txtCvv');
    if (txtCvv) {
        txtCvv.addEventListener('input', (e) => {
            e.target.value = e.target.value.replace(/\s+/g, '').replace(/[^0-9]/gi, '');
        });
    }
});