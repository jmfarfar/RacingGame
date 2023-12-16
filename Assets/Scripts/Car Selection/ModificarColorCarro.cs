using UnityEngine;

public class ModificarColorCarro : MonoBehaviour
{
    public Material materialCarro; // Asigna el material del carro en el Inspector
    public Material[] carMaterial;
    // Método para modificar el color del carro
    public void ModificarColor(Color nuevoColor)
    {
        if (materialCarro != null)
        {
            // Modifica el color del material
            materialCarro.color = nuevoColor;
        }
        else
        {
            Debug.LogError("No se ha asignado un material al carro. Asigna un material en el Inspector.");
        }
    }

    // Método para guardar la variante de color
    public void GuardarVarianteColor()
    {
        if (materialCarro != null)
        {
            // Obtiene el color actual del material
            Color colorActual = materialCarro.color;

            // Puedes guardar el color actual en algún lugar, como PlayerPrefs o un archivo de configuración
            // En este ejemplo, lo imprimimos en la consola
            Debug.Log("Color actual del carro: " + colorActual.ToString());
        }
        else
        {
            Debug.LogError("No se ha asignado un material al carro. Asigna un material en el Inspector.");
        }
    }
}

