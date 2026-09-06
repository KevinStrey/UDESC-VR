using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR; // Necessário para acessar o XRNode e InputDevices

public class XRUIHapticFeedback : MonoBehaviour, IPointerDownHandler
{
    [Tooltip("Intensidade da vibração (0.0 a 1.0)")]
    public float hapticIntensity = 0.5f;
    [Tooltip("Duração da vibração em segundos")]
    public float hapticDuration = 0.1f;

    public void OnPointerDown(PointerEventData eventData)
    {
        // Envia um impulso háptico para os controles quando a UI é pressionada
        SendHaptics();
    }

    private void SendHaptics()
    {
        // Obtém o controle direito e envia a vibração se ele estiver ativo
        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.isValid)
        {
            rightHand.SendHapticImpulse(0, hapticIntensity, hapticDuration);
        }

        // Obtém o controle esquerdo e envia a vibração se ele estiver ativo
        InputDevice leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        if (leftHand.isValid)
        {
            leftHand.SendHapticImpulse(0, hapticIntensity, hapticDuration);
        }
    }
}