(function ($) {
    "use strict";

    // ============================
    // ACTUALIZAR CONTADOR NAVBAR
    // ============================
    function actualizarCarrito() {
        $.ajax({
            url: '/Carritoes/ObtenerCantidad',
            type: 'GET',
            success: function (cantidad) {
                $("#cartCount").text(cantidad);

                if (cantidad === 0) {
                    $("#cartCount").hide();
                } else {
                    $("#cartCount").show();
                }
            }
        });
    }

    // ============================
    // ACTUALIZAR CANTIDAD (general)
    // ============================
    function actualizarCantidad(detalleId, cantidad, input) {
        const fila = input.closest("tr");

        $.ajax({
            url: '/Carritoes/ActualizarCantidad',
            type: 'POST',
            data: {
                detalleId: detalleId,
                cantidad: cantidad,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                console.log("Respuesta del servidor (ActualizarCantidad):", res); // 👈 debug

                // ⚠️ Validación de stock
                if (res.warning) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Stock insuficiente',
                        text: res.message || res.mensaje,
                        timer: 2500,
                        showConfirmButton: false
                    });
                    return; // 👈 detenemos aquí, no actualizamos nada
                }

                if (res && res.success) {
                    if (res.eliminado) {
                        fila.fadeOut(400, function () {
                            $(this).remove();
                            if ($("tbody tr").length === 0) {
                                $(".card").replaceWith(`
                                <div class="alert alert-warning text-center shadow-sm">
                                    <i class="bi bi-exclamation-circle"></i> Tu carrito está vacío.
                                </div>
                            `);
                            }
                        });
                    } else {
                        let subtotal = Number(res.subtotal) || 0;
                        fila.find(".subtotal").text("S/ " + subtotal.toFixed(2));
                    }

                    let total = Number(res.total) || 0;
                    $("#totalCarrito").text("S/ " + total.toFixed(2));
                    actualizarCarrito();
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'La respuesta no fue válida.'
                    });
                }
            },
            error: function (xhr) {
                console.error("Error AJAX:", xhr.responseText); // 👈 debug
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'No se pudo actualizar la cantidad.'
                });
            }
        });
    }

    // ============================
    // AGREGAR AL CARRITO
    // ============================
    $(document).on("click", ".add-to-cart", function () {
        const productoId = $(this).data("id");
        const cantidad = $(this).data("cantidad");

        $.ajax({
            url: '/Carritoes/Agregar',
            type: 'POST',
            data: {
                productoId: productoId,
                cantidad: cantidad,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                if (res.warning) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Stock insuficiente',
                        text: res.mensaje,
                        timer: 2500,
                        showConfirmButton: false
                    });
                    return; // 👈 no sigas
                }

                if (res.success) {
                    actualizarCarrito();
                    Swal.fire({
                        icon: 'success',
                        title: 'Agregado al carrito',
                        text: res.mensaje,
                        timer: 2500,
                        showConfirmButton: false
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Oops...',
                    text: 'No se pudo agregar al carrito.'
                });
            }
        });
    });

    // ============================
    // BOTONES + / -
    // ============================
    $(document).on("click", ".btn-restar", function () {
        let detalleId = $(this).data("id");
        let input = $(this).closest(".input-group").find(".cantidad-input");
        let valor = parseInt(input.val()) || 1;
        let nuevoValor = valor - 1;

        if (nuevoValor <= 0) {
            Swal.fire({
                title: "¿Eliminar producto?",
                text: "Este producto se eliminará del carrito.",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Sí, eliminar",
                cancelButtonText: "Cancelar"
            }).then((result) => {
                if (result.isConfirmed) {
                    actualizarCantidad(detalleId, 0, input);
                }
            });
        } else {
            input.val(nuevoValor);
            actualizarCantidad(detalleId, nuevoValor, input);
        }
    });

    $(document).on("click", ".btn-sumar", function () {
        let detalleId = $(this).data("id");
        let input = $(this).closest(".input-group").find(".cantidad-input");
        let valor = parseInt(input.val()) || 1;
        let nuevoValor = valor + 1;

        input.val(nuevoValor);
        actualizarCantidad(detalleId, nuevoValor, input);
    });

    // ============================
    // ELIMINAR PRODUCTO DIRECTO
    // ============================
    $(document).on("click", ".btn-eliminar", function () {
        const detalleId = $(this).data("id");
        const fila = $(this).closest("tr");

        Swal.fire({
            title: "¿Eliminar producto?",
            text: "Este producto se eliminará del carrito.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/Carritoes/EliminarDetalle',
                    type: 'POST',
                    data: {
                        detalleId: detalleId,
                        __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                    },
                    success: function (res) {

                        if (res.success) {
                            fila.fadeOut(400, function () {
                                $(this).remove();
                                if ($("tbody tr").length === 0) {
                                    $(".card").replaceWith(`
                                        <div class="alert alert-warning text-center shadow-sm">
                                            <i class="bi bi-exclamation-circle"></i> Tu carrito está vacío.
                                        </div>
                                    `);
                                }
                            });
                            $("#totalCarrito").text("S/ " + res.total.toFixed(2));
                            actualizarCarrito();
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo eliminar el producto.'
                        });
                    }
                });
            }
        });
    });

    // ============================
    // CARGAR CANTIDAD INICIAL
    // ============================
    $(document).ready(function () {
        actualizarCarrito();
    });

})(jQuery);


