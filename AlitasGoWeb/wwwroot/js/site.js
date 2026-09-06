
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
