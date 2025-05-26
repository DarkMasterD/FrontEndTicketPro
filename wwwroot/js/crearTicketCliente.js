const baseUrl = "https://localhost:7141"; // ✅ Cambia si tu API usa otro puerto

let archivoParaSubir = null;

function generarCodigo() {
    const prioridad = document.getElementById("prioridad").value;
    if (!prioridad) return alert("Seleccione una prioridad primero.");

    fetch(`${baseUrl}/api/ticket/generar-codigo?prioridad=${prioridad}&tipoUsuario=E`)
        .then(res => {
            if (!res.ok) throw new Error("Error al generar código");
            return res.json();
        })
        .then(data => document.getElementById("codigoTicket").value = data.codigo)
        .catch(() => alert("Error al generar el código del ticket."));
}

function guardarArchivo() {
    archivoParaSubir = document.getElementById("archivoInput").files[0];
    const comentario = document.getElementById("comentarioArchivo").value;

    if (archivoParaSubir) {
        const modal = bootstrap.Modal.getInstance(document.getElementById("modalArchivo"));
        modal.hide();
        alert("Archivo guardado temporalmente. Se subirá al crear el ticket.");
    } else {
        alert("Debe seleccionar un archivo.");
    }
}

document.getElementById("formCrearTicket").addEventListener("submit", async function (event) {
    event.preventDefault();

    const ticket = {
        id_ticket: 0,
        codigo: document.getElementById("codigoTicket").value,
        titulo: document.getElementById("titulo").value,
        descripcion: document.getElementById("descripcion").value,
        servicio: document.getElementById("servicio").value,
        id_categoria_ticket: parseInt(document.getElementById("categoria").value),
        prioridad: document.getElementById("prioridad").value,
        id_usuario: parseInt(document.getElementById("id_usuario").value),
        estado: "No asignado",
        fecha_inicio: new Date().toISOString(),
        tareas: []
    };

    try {
        const response = await fetch(`${baseUrl}/api/ticket`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(ticket)
        });

        if (!response.ok) {
            const errText = await response.text();
            throw new Error("Error al crear ticket: " + errText);
        }

        const data = await response.json();
        const ticketId = data.id;

        if (archivoParaSubir) {
            const formData = new FormData();
            formData.append("archivo", archivoParaSubir);
            formData.append("id_ticket", ticketId);

            const uploadResponse = await fetch(`${baseUrl}/api/ticket/subir-archivo`, {
                method: "POST",
                body: formData
            });

            if (!uploadResponse.ok) {
                console.warn("Archivo no se subió correctamente");
            }
        }

        alert("Ticket creado correctamente.");
        window.location.href = "/Cliente/Inicio";

    } catch (err) {
        console.error("Error:", err);
        alert("Error al crear el ticket. Revise la consola.");
    }
});
