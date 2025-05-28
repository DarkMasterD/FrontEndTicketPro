function validarFormularioContacto() {
    const emailInput = document.querySelector('[name="NuevoContacto.Email"]');
    const telefonoInput = document.querySelector('[name="NuevoContacto.Telefono"]');

    const email = emailInput ? emailInput.value.trim() : "";
    const telefono = telefonoInput ? telefonoInput.value.trim() : "";

    const emailRegex = /^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$/;
    const telefonoRegex = /^\\d{4}-\\d{4}$/;

    const emailValido = email === "" || emailRegex.test(email);
    const telValido = telefono === "" || telefonoRegex.test(telefono);

    if (!emailValido) {
        alert("Ingrese un correo válido (ej: ejemplo@dominio.com).");
        return false;
    }

    if (!telValido) {
        alert("Ingrese un número de teléfono válido (formato 9999-9999).");
        return false;
    }

    return true;
}