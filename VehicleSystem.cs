using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class VehicleSystem : MonoBehaviour
{
    [Header("Referensi Kendaraan")]
    public GameObject player;      
    public GameObject car;         
    public Transform carSeatPosition; 
    
    public GameObject bike;        
    public Transform bikeSeatPosition; 

    [Header("Referensi Kamera")]
    public GameObject playerCamera; 
    public GameObject carCamera;    
    public GameObject bikeCamera;  

    [Header("Referensi UI Mobile")]
    public GameObject btnInteraksi; 

    private bool isInVehicle = false;
    private bool isInBike = false;
    
    private ThirdPersonController playerController;
    private CharacterController charController;
    private PlayerInput playerInput; // Untuk mematikan input saat naik kendaraan

    void Start()
    {
        playerController = player.GetComponent<ThirdPersonController>();
        charController = player.GetComponent<CharacterController>();
        playerInput = player.GetComponent<PlayerInput>(); 
        
        if (car.GetComponent<CarJoystickController>() != null) car.GetComponent<CarJoystickController>().enabled = false;
        if (bike.GetComponent<BikeController>() != null) bike.GetComponent<BikeController>().enabled = false;
        
        carCamera.SetActive(false); 
        if(bikeCamera != null) bikeCamera.SetActive(false);
        if(btnInteraksi != null) btnInteraksi.SetActive(false); 
    }

    void Update()
    {
        if (isInVehicle) 
        {
            // Kunci posisi karakter di sadel sepeda
            if (isInBike && bikeSeatPosition != null)
            {
                player.transform.position = bikeSeatPosition.position;
                player.transform.rotation = bikeSeatPosition.rotation;
            }

            if (btnInteraksi != null) btnInteraksi.SetActive(true);

            // Tombol E untuk turun
            if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
            {
                ExitVehicle();
            }
            return;
        }

        float distToCar = Vector3.Distance(player.transform.position, car.transform.position);
        float distToBike = Vector3.Distance(player.transform.position, bike.transform.position);

        // Tampilkan tombol UI jika dekat
        if (distToCar <= 4f || distToBike <= 3f)
        {
            if (btnInteraksi != null) btnInteraksi.SetActive(true); 
        }
        else
        {
            if (btnInteraksi != null) btnInteraksi.SetActive(false); 
        }

        // Tombol E untuk masuk
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (distToCar <= 4f) EnterCar();
            else if (distToBike <= 3f) EnterBike();
        }
    }

    // Fungsi ini dipanggil oleh Tombol UI saat di HP
    public void TombolAksiDitekan()
    {
        if (isInVehicle)
        {
            ExitVehicle();
        }
        else
        {
            float distToCar = Vector3.Distance(player.transform.position, car.transform.position);
            float distToBike = Vector3.Distance(player.transform.position, bike.transform.position);

            if (distToCar <= 4f) EnterCar();
            else if (distToBike <= 3f) EnterBike();
        }
    }

    void EnterCar()
    {
        isInVehicle = true; isInBike = false;
        playerController.enabled = false; 
        charController.enabled = false; 
        if(playerInput != null) playerInput.enabled = false; // Matikan input
        
        player.transform.SetParent(car.transform); 
        player.transform.position = carSeatPosition.position;
        player.transform.rotation = carSeatPosition.rotation;
        
        car.GetComponent<CarJoystickController>().enabled = true;
        playerCamera.SetActive(false); carCamera.SetActive(true);
    }

    void EnterBike()
    {
        isInVehicle = true; isInBike = true;
        playerController.enabled = false; 
        charController.enabled = false; 
        if(playerInput != null) playerInput.enabled = false; 
        
        player.transform.SetParent(bike.transform); 
        player.transform.position = bikeSeatPosition.position;
        player.transform.rotation = bikeSeatPosition.rotation;
        
        bike.GetComponent<BikeController>().enabled = true;
        
        playerCamera.SetActive(false); 
        if (bikeCamera != null) bikeCamera.SetActive(true);
    }

    void ExitVehicle()
    {
        isInVehicle = false;
        player.transform.SetParent(null);
        
        if (isInBike) {
            player.transform.position = bike.transform.position + (bike.transform.right * -1.5f) + (Vector3.up * 1f);
            bike.GetComponent<BikeController>().enabled = false;
            if (bikeCamera != null) bikeCamera.SetActive(false);
        } else {
            player.transform.position = car.transform.position + (car.transform.right * -2f) + (Vector3.up * 1f);
            car.GetComponent<CarJoystickController>().enabled = false;
            carCamera.SetActive(false);
        }

        playerController.enabled = true; 
        charController.enabled = true;
        if(playerInput != null) playerInput.enabled = true; // Nyalakan input lagi
        
        playerCamera.SetActive(true);
        if (btnInteraksi != null) btnInteraksi.SetActive(false);
    }
}
