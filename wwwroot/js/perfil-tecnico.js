const idUsuario = document.getElementById("id_usuario").value;

// ============ ACTUALIZAR PERFIL ============
document.getElementById("formActualizarPerfil").addEventListener("submit", async function (e) {
    e.preventDefault();

    const formData = new FormData(e.target);
    const payload = {
        IdUsuario: parseInt(idUsuario),
        Usuario: formData.get("Usuario"),
        Direccion: formData.get("Direccion"),
        NombreCompleto: formData.get("NombreCompleto")
    };

    try {
        const res = await fetch("https://localhost:7141/api/usuario/actualizar-perfil", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const result = await res.text();
        alert(result);
        if (res.ok) location.reload();
    } catch (error) {
        console.error("Error al actualizar perfil:", error);
        alert("Error al actualizar el perfil");
    }
});

// ============ CAMBIAR CONTRASEÑA ============
document.getElementById("formCambiarContrasenia").addEventListener("submit", async function (e) {
    e.preventDefault();

    const actual = document.getElementById("actual").value;
    const nueva = document.getElementById("nueva").value;
    const confirmar = document.getElementById("confirmar").value;

    if (nueva !== confirmar) {
        alert("Las contraseñas no coinciden");
        return;
    }

    const payload = {
        idUsuario: parseInt(idUsuario),
        actual: actual,
        nueva: nueva
    };

    try {
        const res = await fetch("https://localhost:7141/api/usuario/cambiar-contrasenia", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        const result = await res.text();
        alert(result);

        if (res.ok) {
            e.target.reset();
            bootstrap.Modal.getInstance(document.getElementById("modalContrasenia")).hide();
        }
    } catch (error) {
        console.error("Error al cambiar contraseña:", error);
        alert("Error al cambiar la contraseña");
    }
});

// ============ GESTIÓN DE CONTACTOS ============

const renderContacto = (tipo, valor, id = null) => {
    return `
        <div class="input-group mb-2 contacto-item" data-id="${id || ''}" data-tipo="${tipo}">
            <input class="form-control" value="${valor}" readonly />
            <button class="btn btn-outline-primary editar-contacto" type="button">
                <i class="bi bi-pencil-square"></i>
            </button>
        </div>`;
};

// Agregar teléfono
document.getElementById("btnNuevoTelefono").addEventListener("click", () => {
    const html = `
        <div class="input-group mb-2 nuevo-contacto">
            <input type="text" class="form-control" placeholder="Ej: 7777-8888" required />
            <button class="btn btn-success guardar-contacto" data-tipo="telefono">💾</button>
            <button class="btn btn-outline-secondary cancelar-contacto">✖️</button>
        </div>`;
    document.getElementById("listaTelefonos").insertAdjacentHTML("beforeend", html);
});

// Agregar correo
document.getElementById("btnNuevoCorreo").addEventListener("click", () => {
    const html = `
        <div class="input-group mb-2 nuevo-contacto">
            <input type="email" class="form-control" placeholder="Ej: correo@ejemplo.com" required />
            <button class="btn btn-success guardar-contacto" data-tipo="correo">💾</button>
            <button class="btn btn-outline-secondary cancelar-contacto">✖️</button>
        </div>`;
    document.getElementById("listaCorreos").insertAdjacentHTML("beforeend", html);
});

// Delegación de eventos para contactos
document.addEventListener("click", async function (e) {
    // Guardar contacto nuevo
    if (e.target.classList.contains("guardar-contacto")) {
        const tipo = e.target.dataset.tipo;
        const inputGroup = e.target.closest(".nuevo-contacto");
        const input = inputGroup.querySelector("input");
        const nuevoValor = input.value.trim();

        if (tipo === "telefono" && !/^\d{4}-\d{4}$/.test(nuevoValor)) {
            alert("Formato de teléfono inválido (use 9999-9999)");
            return;
        }

        if (tipo === "correo" && !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(nuevoValor)) {
            alert("Correo electrónico inválido");
            return;
        }

        const payload = [{
            IdUsuario: parseInt(idUsuario),
            Telefono: tipo === "telefono" ? nuevoValor : "",
            Email: tipo === "correo" ? nuevoValor : ""
        }];

        try {
            const res = await fetch("https://localhost:7141/api/usuario/agregar-contactos", {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(payload)
            });

            if (res.ok) {
                await res.text();
                inputGroup.outerHTML = renderContacto(tipo, nuevoValor);
                alert("Contacto agregado correctamente");
            }
        } catch (error) {
            console.error("Error al agregar contacto:", error);
            alert("Error al agregar el contacto");
        }
    }

    // Cancelar contacto nuevo
    if (e.target.classList.contains("cancelar-contacto")) {
        e.target.closest(".nuevo-contacto").remove();
    }

    // Editar contacto existente
    // Editar contacto existente
    if (e.target.closest(".editar-contacto")) {
        const item = e.target.closest(".contacto-item");
        const input = item.querySelector("input");
        const tipo = item.dataset.tipo;
        const id = item.dataset.id;
        const originalValue = input.value;

        // Quitar readonly y reemplazar botón por guardar/cancelar
        input.removeAttribute("readonly");

        // Ocultar botón de editar
        item.querySelector(".editar-contacto").style.display = "none";

        // Agregar botones de guardar y cancelar
        const btnGuardar = document.createElement("button");
        btnGuardar.className = "btn btn-outline-success btn-guardar-contacto";
        btnGuardar.innerHTML = "✅";
        btnGuardar.type = "button";

        const btnCancelar = document.createElement("button");
        btnCancelar.className = "btn btn-outline-secondary btn-cancelar-contacto";
        btnCancelar.innerHTML = "❌";
        btnCancelar.type = "button";

        item.appendChild(btnGuardar);
        item.appendChild(btnCancelar);

        // Guardar
        btnGuardar.addEventListener("click", async () => {
            const nuevoValor = input.value.trim();

            // Validaciones
            if (tipo === "telefono" && !/^\d{4}-\d{4}$/.test(nuevoValor)) {
                alert("Formato inválido. Usa 9999-9999");
                return;
            }
            if (tipo === "correo" && !/^[^@\s]+@[^@\s]+\.[^@\s]+$/.test(nuevoValor)) {
                alert("Correo electrónico inválido");
                return;
            }

            const payload = {
                Id: parseInt(id),
                Telefono: tipo === "telefono" ? nuevoValor : "",
                Email: tipo === "correo" ? nuevoValor : ""
            };

            try {
                const res = await fetch("https://localhost:7141/api/usuario/actualizar-contacto", {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    body: JSON.stringify(payload)
                });

                if (res.ok) {
                    alert("Contacto actualizado correctamente");
                    input.setAttribute("readonly", true);
                    btnGuardar.remove();
                    btnCancelar.remove();
                    item.querySelector(".editar-contacto").style.display = "";
                } else {
                    throw new Error("Error al actualizar");
                }
            } catch (error) {
                alert("Error al actualizar contacto");
                console.error(error);
            }
        });

        // Cancelar
        btnCancelar.addEventListener("click", () => {
            input.value = originalValue;
            input.setAttribute("readonly", true);
            btnGuardar.remove();
            btnCancelar.remove();
            item.querySelector(".editar-contacto").style.display = "";
        });
    }

});