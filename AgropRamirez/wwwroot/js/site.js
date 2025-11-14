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
                        // 🔹 Actualizar cantidad
                        input.val(cantidad);

                        // 🟢 Actualizar precio unitario si viene del servidor
                        if (res.precioUnitario) {
                            const precioUnitario = Number(res.precioUnitario);
                            const precioTexto = "S/ " + precioUnitario.toFixed(2);

                            // Cambiar el texto en tiempo real con un efecto suave
                            fila.find(".precio-unitario")
                                .fadeOut(100)
                                .text(precioTexto)
                                .fadeIn(100);

                            // 🔁 Recalcular subtotal en el cliente
                            const nuevoSubtotal = precioUnitario * cantidad;
                            fila.find(".subtotal").text("S/ " + nuevoSubtotal.toFixed(2));
                        } else {
                            // Si el servidor no devuelve precio, usar el subtotal que sí devuelve
                            let subtotal = Number(res.subtotal) || 0;
                            fila.find(".subtotal").text("S/ " + subtotal.toFixed(2));
                        }
                    
                    }

                    let total = Number(res.total) || 0;
                    $("#totalCarrito").text("S/ " + total.toFixed(2));
                    actualizarCarrito();

                    // 🟢 NUEVO: Mostrar mensaje informativo (precio mayorista / normal)
                    if (res.mensaje && res.mensaje.length > 0) {
                        Swal.fire({
                            toast: true,
                            position: 'top-end',
                            icon: 'info',
                            title: res.mensaje,
                            showConfirmButton: false,
                            timer: 2000,
                            timerProgressBar: true
                        });
                    }

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

        // ✅ Leer desde los atributos de Razor inyectados
        const isAuthenticated = $(this).data("authenticated") === true || $(this).data("authenticated") === "True";
        const isAdmin = $(this).data("admin") === true || $(this).data("admin") === "True";

        // 🔒 No logueado
        if (!isAuthenticated) {
            Swal.fire({
                icon: 'info',
                title: 'Inicia sesión',
                text: 'Debes iniciar sesión para agregar productos al carrito.',
                confirmButtonText: 'Ir al inicio de sesión',
                confirmButtonColor: '#ffc107'
            }).then((result) => {
                if (result.isConfirmed) {
                    window.location.href = '/Cuenta/Login';
                }
            });
            return;
        }

        // ⚙️ Administrador
        if (isAdmin) {
            window.location.href = '/Carritoes/Create';
            return;
        }

        // 🔸 3. Cliente logueado → Ejecutar AJAX normal
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
                            // 💨 Animación al eliminar la fila
                            fila.fadeOut(400, function () {
                                $(this).remove();

                                // 🔍 Verificamos si ya no hay productos ni promociones
                                const hayProductos = $(".carrito-productos tr[data-tipo='producto']").length > 0;
                                const hayPromos = $(".carrito-promociones tr[data-tipo='promocion']").length > 0;

                                if (!hayProductos && !hayPromos) {
                                    $(".card").fadeOut(300, function () {
                                        $(this).replaceWith(`
                                        <div id="carrito-vacio" class="text-center py-5">
                                            <img src="/img/CarritoVacio.png" alt="Carrito vacío"
                                                 class="img-fluid mb-3" style="max-width: 260px; opacity: 0.9;">
                                            <h5 class="text-muted fw-semibold">
                                                <i class="bi bi-emoji-frown"></i> Tu carrito está vacío.
                                            </h5>
                                            <p class="text-secondary mb-4">Parece que aún no has agregado productos ni promociones.</p>
                                            <a href="/Productoes/Index" class="btn btn-warning text-dark fw-bold shadow-sm px-4">
                                                <i class="bi bi-shop me-1"></i> Ir a comprar
                                            </a>
                                        </div>
                                    `).hide().fadeIn(500);
                                    });
                                }
                            });

                            // 💰 Actualizamos el total mostrado
                            $("#totalCarrito").text("S/ " + res.total.toFixed(2));

                            // 🔁 Actualiza el ícono o contador global del carrito
                            if (typeof actualizarCarrito === "function") {
                                actualizarCarrito();
                            }
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


    // =====================================
    // 🔔 CARGAR NOTIFICACIONES DINÁMICAS
    // =====================================
    function cargarNotificaciones() {
        $.ajax({
            url: '/Notificacions/NotificacionesNavbar',
            type: 'GET',
            cache: false,
            success: function (html) {
                $('#navbarNotificacionesContainer').html(html);
            },
            error: function (xhr, status, error) {
                console.error('Error al cargar notificaciones:', error);
            }
        });
    }

    // ============================
    // CARGAR CANTIDAD INICIAL
    // ============================
    $(document).ready(function () {
        actualizarCarrito();
        cargarNotificaciones();

        setInterval(cargarNotificaciones, 60000);    // Notificaciones cada minuto
    });

    //Añadir promocion al carrito
    $(document).on("click", ".btn-add-promo", function (event) {
        event.stopPropagation(); // 👈 evita que el clic del card se dispare
        event.preventDefault();  // 🔹 Evita comportamiento por defecto (seguridad extra)
        const promoId = $(this).data("id");

        $.ajax({
            url: '/Carritoes/AgregarPromocionCompacta',
            type: 'POST',
            data: {
                promocionId: promoId,
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (res) {
                if (res.warning) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Atención',
                        text: res.mensaje,
                        timer: 2500,
                        showConfirmButton: false
                    });
                    return;
                }

                if (res.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Promoción añadida',
                        text: res.mensaje,
                        timer: 2000,
                        showConfirmButton: false
                    });
                    actualizarCarrito(); // 👈 refresca el contador del carrito en el navbar
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: res.mensaje || 'No se pudo agregar la promoción.'
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error del servidor',
                    text: 'No se pudo procesar tu solicitud.'
                });
            }
        });
    });

    // ============================
    // ELIMINAR PROMOCIÓN DEL CARRITO
    // ============================
    $(document).on("click", ".btn-eliminar-promocion", function () {
        const promoRow = $(this).closest("tr");
        const carritoPromocionId = $(this).data("id");

        Swal.fire({
            title: "¿Eliminar promoción?",
            text: "Esta promoción será eliminada del carrito.",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Sí, eliminar",
            cancelButtonText: "Cancelar"
        }).then((result) => {
            if (!result.isConfirmed) return;

            $.ajax({
                url: '/Carritoes/EliminarPromocion',
                type: 'POST',
                data: {
                    carritoPromocionId: carritoPromocionId,
                    __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
                },
                success: function (res) {
                    if (!res || !res.success) {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: res?.message || 'No se pudo eliminar la promoción.'
                        });
                        return;
                    }

                    // 💨 Animación al eliminar fila
                    promoRow.fadeOut(300, function () {
                        $(this).remove();

                        // 🔍 Verificar si el carrito quedó vacío
                        const hayProductos = $(".carrito-productos tr[data-tipo='producto']").length > 0;
                        const hayPromos = $(".carrito-promociones tr[data-tipo='promocion']").length > 0;

                        if (!hayProductos && !hayPromos) {
                            $(".card").fadeOut(300, function () {
                                $(this).replaceWith(`
                                <div id="carrito-vacio" class="text-center py-5">
                                    <img src="/img/CarritoVacio.png" alt="Carrito vacío"
                                         class="img-fluid mb-3" style="max-width: 260px; opacity: 0.9;">
                                    <h5 class="text-muted fw-semibold">
                                        <i class="bi bi-emoji-frown"></i> Tu carrito está vacío.
                                    </h5>
                                    <p class="text-secondary mb-4">
                                        Parece que aún no has agregado productos ni promociones.
                                    </p>
                                    <a href="/Productoes/Index"
                                       class="btn btn-warning text-dark fw-bold shadow-sm px-4">
                                        <i class="bi bi-shop me-1"></i> Ir a comprar
                                    </a>
                                </div>
                            `).hide().fadeIn(500);
                            });
                        }
                    });

                    // 💰 Actualizar total mostrado
                    const total = Number(res.total || 0);
                    $("#totalCarrito").text("S/ " + total.toFixed(2));

                    // 🔁 Actualizar contador del navbar si existe
                    if (typeof actualizarCarrito === "function") {
                        actualizarCarrito();
                    }

                    // ✅ Confirmación visual
                    Swal.fire({
                        icon: 'success',
                        title: 'Eliminado',
                        text: 'La promoción fue eliminada del carrito.',
                        timer: 1500,
                        showConfirmButton: false
                    });
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error del servidor',
                        text: 'No se pudo procesar tu solicitud.'
                    });
                }
            });
        });
    });


    


})(jQuery);


