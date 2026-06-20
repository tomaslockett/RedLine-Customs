function setupTogglePassword(iconId, inputId)
{
    const icon = document.getElementById(iconId);
    const input = document.getElementById(inputId);

    if (!icon || !input) return;

    icon.addEventListener('click', function ()
    {
        const type = input.getAttribute('type') === 'password' ? 'text' : 'password';
        input.setAttribute('type', type);

        if (type === 'text')
        {
            this.style.color = "white";
            this.classList.replace('fa-eye-slash', 'fa-eye');
        }
        else
        {
            this.style.color = "rgba(255,255,255,0.3)";
            this.classList.replace('fa-eye', 'fa-eye-slash');
        }
    });
}