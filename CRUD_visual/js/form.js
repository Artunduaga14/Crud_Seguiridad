document.getElementById("formulario").addEventListener("submit", async function(event) {
    event.preventDefault();


    const name = document.getElementById("formName").value;
    const description = document.getElementById("formDescription").value;

    const data = {
        name:name,
        description:description
    }

    try{
        const response = await fetch("http://localhost:5150/api/Form", {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify(data),
          });
    
          if (response.ok) {
            ListaPermisos();
            alert("Registro exitoso");
          } else {
            alert("Error en el registro");
          }
        } catch (error) {
          console.error("Error:", error);
          alert("Error en el registro");
        }
      });


    "ingresar datos"
    const tabla = document.getElementById("Tabla");
    const fila = tabla.iner();

    const celdaId = fila.insertCell(0);
    const celdaNombre = fila.insertCell(1);
    const celdaDescripcion = fila.insertCell(2);
