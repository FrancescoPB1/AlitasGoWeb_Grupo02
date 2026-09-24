
(function () {
    "use strict";

    var modal = document.getElementById("modalEliminar");
    if (modal) {
        modal.addEventListener("show.bs.modal", function (event) {
            var boton = event.relatedTarget;
            if (!boton) return;

            var form = modal.querySelector("#formEliminar");
            form.action = boton.getAttribute("data-url") || "";

            var nombre = modal.querySelector("#modalEliminarNombre");
            nombre.textContent = boton.getAttribute("data-nombre") || "este registro";

            var detalle = modal.querySelector("#modalEliminarDetalle");
            detalle.textContent = boton.getAttribute("data-detalle") || "";
        });
    }

    document.querySelectorAll(".js-alert-auto").forEach(function (alerta) {
        setTimeout(function () {
            var instancia = bootstrap.Alert.getOrCreateInstance(alerta);
            instancia.close();
        }, 5000);
    });

    document.querySelectorAll("form[data-una-vez]").forEach(function (form) {
        form.addEventListener("submit", function () {
            if (window.jQuery && jQuery(form).valid && !jQuery(form).valid()) return;
            var btn = form.querySelector("button[type=submit]");
            if (btn) {
                btn.disabled = true;
                btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1" role="status" aria-hidden="true"></span>Guardando…';
            }
        });
    });
})();

// ---------- Filas dinámicas (maestro-detalle) ----------
// El model binding de MVC se corta si los índices tienen huecos (Detalles[0], Detalles[2]):
// por eso, al quitar una fila, se renumeran todas.
window.AlitasGo = window.AlitasGo || {};
AlitasGo.filasDinamicas = function (opciones) {
    "use strict";
    var cuerpo = document.getElementById(opciones.cuerpo);
    var plantilla = document.getElementById(opciones.plantilla);
    var prefijo = opciones.prefijo;
    if (!cuerpo || !plantilla) return;

    var reNombre = new RegExp("^" + prefijo + "\\[\\d+\\]");
    var reId = new RegExp("^" + prefijo + "_\\d+__");

    function reindexar() {
        cuerpo.querySelectorAll("[data-fila]").forEach(function (fila, i) {
            fila.querySelectorAll("[name]").forEach(function (el) {
                el.name = el.name.replace(reNombre, prefijo + "[" + i + "]");
            });
            fila.querySelectorAll("[id]").forEach(function (el) {
                el.id = el.id.replace(reId, prefijo + "_" + i + "__");
            });
            fila.querySelectorAll("label[for]").forEach(function (el) {
                el.htmlFor = el.htmlFor.replace(reId, prefijo + "_" + i + "__");
            });
        });
    }

    document.getElementById(opciones.botonAgregar).addEventListener("click", function () {
        var n = cuerpo.querySelectorAll("[data-fila]").length;
        cuerpo.insertAdjacentHTML("beforeend", plantilla.innerHTML.replace(/__i__/g, n));
        var nueva = cuerpo.querySelectorAll("[data-fila]")[n];
        if (opciones.alAgregar) opciones.alAgregar(nueva);
        var primero = nueva.querySelector("select, input:not([type=hidden])");
        if (primero) primero.focus();
    });

    cuerpo.addEventListener("click", function (e) {
        var boton = e.target.closest(".js-quitar-fila");
        if (!boton) return;
        var filas = cuerpo.querySelectorAll("[data-fila]");
        if (filas.length <= 1) return;   // siempre queda al menos una fila
        boton.closest("[data-fila]").remove();
        reindexar();
    });
};

// ---------- Tiempo real (SignalR) ----------
// Una sola conexión por página. Cada aviso se reenvía como evento "alitasgo:pedido"
// para que cada pantalla decida qué refrescar.
AlitasGo.tiempoReal = (function () {
    "use strict";
    if (!window.signalR || document.body.getAttribute("data-tiempo-real") !== "si") return null;

    var indicador = document.getElementById("indicadorTiempoReal");
    function estado(texto, clase) {
        if (!indicador) return;
        indicador.textContent = "● " + texto;
        indicador.className = "badge " + clase;
    }

    function mostrarAviso(mensaje) {
        if (document.body.getAttribute("data-toast") === "no") return;
        var contenedor = document.getElementById("avisosTiempoReal");
        if (!contenedor || !window.bootstrap) return;
        var toast = document.createElement("div");
        toast.className = "toast align-items-center text-bg-dark border-0";
        toast.setAttribute("role", "status");
        toast.innerHTML = '<div class="d-flex"><div class="toast-body fs-6"></div>' +
            '<button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Cerrar"></button></div>';
        toast.querySelector(".toast-body").textContent = mensaje;   // textContent: sin inyección de HTML
        contenedor.appendChild(toast);
        toast.addEventListener("hidden.bs.toast", function () { toast.remove(); });
        new bootstrap.Toast(toast, { delay: 8000 }).show();
    }

    var conexion = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/pedidos")
        .withAutomaticReconnect()
        .build();

    conexion.on("pedidoActualizado", function (aviso) {
        document.dispatchEvent(new CustomEvent("alitasgo:pedido", { detail: aviso }));
        if (aviso.mensaje) mostrarAviso(aviso.mensaje);
    });
    conexion.onreconnecting(function () { estado("Reconectando…", "bg-warning text-dark"); });
    conexion.onreconnected(function () {
        estado("En vivo", "bg-success");
        document.dispatchEvent(new CustomEvent("alitasgo:reconectado"));   // pudo perderse algún aviso
    });
    conexion.onclose(function () { estado("Sin conexión", "bg-danger"); setTimeout(iniciar, 5000); });

    function iniciar() {
        conexion.start()
            .then(function () { estado("En vivo", "bg-success"); })
            .catch(function () { estado("Sin conexión", "bg-danger"); setTimeout(iniciar, 5000); });
    }
    iniciar();
    return conexion;
})();

// Vuelve a pedir un bloque parcial (salón, reparto, lista de pedidos) cuando llega un aviso.
AlitasGo.refrescarAlCambiar = function (idContenedor, obtenerUrl, filtro) {
    "use strict";
    var contenedor = document.getElementById(idContenedor);
    if (!contenedor) return null;
    var enCurso = false, pendiente = false;

    function refrescar() {
        if (enCurso) { pendiente = true; return; }
        enCurso = true;
        $.get(obtenerUrl())
            .done(function (html) {
                // Si la sesión expiró llega la página de login: se recarga para mostrarla.
                if (html.indexOf("data-parcial") === -1) { location.reload(); return; }
                contenedor.innerHTML = html;
            })
            .always(function () {
                enCurso = false;
                if (pendiente) { pendiente = false; refrescar(); }
            });
    }

    document.addEventListener("alitasgo:pedido", function (e) { if (!filtro || filtro(e.detail)) refrescar(); });
    document.addEventListener("alitasgo:reconectado", refrescar);
    return refrescar;
};
