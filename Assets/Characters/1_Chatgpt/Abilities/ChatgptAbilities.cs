using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;

public class ChatgptAbilities : NetworkBehaviour
{
    [SerializeField] private Transform shootTransform;
    [SerializeField] private Canvas abilitiesCanvas;
    private PlayerMovement playerMovement;
    private PlayerPrefab stats;

    [Header("Ability 1")]
    [SerializeField] private GameObject ability1Projectile;
    public Image ability1Image;
    public TMP_Text ability1Text;
    public KeyCode ability1Key = KeyCode.Q;
    public float ability1Cooldown;
    public Canvas ability1Canvas;
    public Image ability1Indicator;
    public GameObject ability1DisableOverlay;
    public float CYBERBALL_DAMAGE = 10;

    [Header("Ability 2")]
    [SerializeField] private GameObject ability2Projectile;
    public Image ability2Image;
    public TMP_Text ability2Text;
    public KeyCode ability2Key = KeyCode.W;
    public float ability2Cooldown;
    public Canvas ability2Canvas;
    public Image ability2Indicator;
    public GameObject ability2DisableOverlay;
    public float AT_CAPACITY_ROOT_DURATION = 2f;

    [Header("Ability 3")]
    public Image ability3Image;
    public TMP_Text ability3Text;
    public KeyCode ability3Key = KeyCode.E;
    public float ability3Cooldown;
    public GameObject ability3DisableOverlay;
    public float NATURAL_LANGUAGE_PROCESSING_SPEED_DURATION = 2f;

    [Header("Ability 4")]
    [SerializeField] private GameObject ability4Projectile;
    public Image ability4Image;
    public TMP_Text ability4Text;
    public KeyCode ability4Key = KeyCode.R;
    public float ability4Cooldown;
    public Canvas ability4Canvas;
    public Image ability4Indicator;
    public GameObject ability4DisableOverlay;

    private bool isAbility1Cooldown = false;
    private bool isAbility2Cooldown = false;
    private bool isAbility3Cooldown = false;
    private bool isAbility4Cooldown = false;

    private float currentAbility1Cooldown;
    private float currentAbility2Cooldown;
    private float currentAbility3Cooldown;
    private float currentAbility4Cooldown;

    private Vector3 position;
    private RaycastHit hit;
    private Ray ray;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        stats = GetComponent<PlayerPrefab>();

        // Shows UI
        if (IsOwner)
        {
            abilitiesCanvas.gameObject.SetActive(true);
            ability1Canvas.gameObject.SetActive(true);
            ability2Canvas.gameObject.SetActive(true);
            ability4Canvas.gameObject.SetActive(true);
        }

        ability1Image.fillAmount = 0;
        ability2Image.fillAmount = 0;
        ability3Image.fillAmount = 0;
        ability4Image.fillAmount = 0;

        ability1Text.text = "";
        ability2Text.text = "";
        ability3Text.text = "";
        ability4Text.text = "";

        ability1Indicator.enabled = false;
        ability2Indicator.enabled = false;
        ability4Indicator.enabled = false;

        ability1Canvas.enabled = false;
        ability2Canvas.enabled = false;
        ability4Canvas.enabled = false;
    }

    void Update()
    {
        if (!IsOwner) { return; }
        if (stats.IsSilenced)
        {
            ability1DisableOverlay.SetActive(true);
            ability2DisableOverlay.SetActive(true);
            ability3DisableOverlay.SetActive(true);
            ability4DisableOverlay.SetActive(true);
            return;
        }
        ability1DisableOverlay.SetActive(false);
        ability2DisableOverlay.SetActive(false);
        ability3DisableOverlay.SetActive(false);
        ability4DisableOverlay.SetActive(false);

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Ability1Input();
        Ability2Input();
        Ability3Input();
        Ability4Input();

        AbilityCooldown(ref currentAbility1Cooldown, ability1Cooldown, ref isAbility1Cooldown, ability1Image, ability1Text);
        AbilityCooldown(ref currentAbility2Cooldown, ability2Cooldown, ref isAbility2Cooldown, ability2Image, ability2Text);
        AbilityCooldown(ref currentAbility3Cooldown, ability3Cooldown, ref isAbility3Cooldown, ability3Image, ability3Text);
        AbilityCooldown(ref currentAbility4Cooldown, ability4Cooldown, ref isAbility4Cooldown, ability4Image, ability4Text);

        Ability1Canvas();
        Ability2Canvas();
        Ability4Canvas();
    }

    private void Ability1Canvas()
    {
        if (ability1Indicator.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            Quaternion ab1Canvas = Quaternion.LookRotation(position - transform.position);
            ab1Canvas.eulerAngles = new Vector3(0, ab1Canvas.eulerAngles.y, ab1Canvas.eulerAngles.z);

            ability1Canvas.transform.rotation = Quaternion.Lerp(ab1Canvas, ability1Canvas.transform.rotation, 0);
        }
    }

    private void Ability2Canvas()
    {
        if (ability2Indicator.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            Quaternion ab2Canvas = Quaternion.LookRotation(position - transform.position);
            ab2Canvas.eulerAngles = new Vector3(0, ab2Canvas.eulerAngles.y, ab2Canvas.eulerAngles.z);

            ability2Canvas.transform.rotation = Quaternion.Lerp(ab2Canvas, ability2Canvas.transform.rotation, 0);
        }
    }

    private void Ability4Canvas()
    {
        if (ability4Indicator.enabled)
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
            Quaternion ab4Canvas = Quaternion.LookRotation(position - transform.position);
            ab4Canvas.eulerAngles = new Vector3(0, ab4Canvas.eulerAngles.y, ab4Canvas.eulerAngles.z);

            ability4Canvas.transform.rotation = Quaternion.Lerp(ab4Canvas, ability4Canvas.transform.rotation, 0);
        }
    }

    private void Ability1Input()
    {
        if (Input.GetKeyDown(ability1Key) && !isAbility1Cooldown)
        {
            ability1Canvas.enabled = true;
            ability1Indicator.enabled = true;

            ability2Canvas.enabled = false;
            ability2Indicator.enabled = false;

            ability4Canvas.enabled = false;
            ability4Indicator.enabled = false;
        }

        if (ability1Canvas.enabled && Input.GetKeyUp(ability1Key))
        {
            // There was a bug with the raycast hit hitting the player prefab and using that y value, 
            // which sends the projectile into the air cuz the click was on top of a guy. There r prob 2 ways to solve this,
            // either set the layer mask to only get the ground layer OR dont use the hit's y value
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                playerMovement.StopMovement();
                playerMovement.Rotate(hit.point);
                CastCyberballServerRpc(hit.point, Quaternion.LookRotation(new Vector3(hit.point.x, 0, hit.point.z) - transform.position));
            }

            isAbility1Cooldown = true;
            currentAbility1Cooldown = ability1Cooldown;

            ability1Canvas.enabled = false;
            ability1Indicator.enabled = false;

            // SetTrigger does not work on network animator unless actual component is called
            GetComponent<OwnerNetworkAnimator>().SetTrigger("CastCyberball");
        }
    }

    [ServerRpc]
    private void CastCyberballServerRpc(Vector3 pos, Quaternion rot)
    {
        playerMovement.Rotate(pos);
        GameObject go = Instantiate(ability1Projectile, new Vector3(shootTransform.position.x, shootTransform.position.y, shootTransform.position.z), rot);
        Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>());
        go.GetComponent<MoveChatgptCyberball>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();
    }

    private void Ability2Input()
    {
        if (Input.GetKeyDown(ability2Key) && !isAbility2Cooldown)
        {
            ability2Canvas.enabled = true;
            ability2Indicator.enabled = true;

            ability1Canvas.enabled = false;
            ability1Indicator.enabled = false;

            ability4Canvas.enabled = false;
            ability4Indicator.enabled = false;

        }
        if (ability2Canvas.enabled && Input.GetKeyUp(ability2Key))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                playerMovement.StopMovement();
                playerMovement.Rotate(hit.point);
                CastAbility2ServerRpc(Quaternion.LookRotation(new Vector3(hit.point.x, 0, hit.point.z) - transform.position));
                foreach (GameObject player in GameManager.Instance.playerPrefabs)
                {
                    if (player == gameObject) { continue; }
                    Vector3 directionToTarget = transform.position - player.transform.position;
                    float angle = Vector3.Angle(transform.forward, directionToTarget);
                    float distance = directionToTarget.magnitude;
                    Debug.Log(Mathf.Abs(angle));
                    Debug.Log(directionToTarget);
                    Debug.Log(distance);

                    // Target is in front of me
                    if (Mathf.Abs(angle) > 130 && distance < 4.5)
                    {
                        GameManager.Instance.Root(player, AT_CAPACITY_ROOT_DURATION);
                    }
                }
            }
            isAbility2Cooldown = true;
            currentAbility2Cooldown = ability2Cooldown;

            ability2Canvas.enabled = false;
            ability2Indicator.enabled = false;

            GetComponent<OwnerNetworkAnimator>().SetTrigger("CastHack");
        }
    }

    [ServerRpc]
    private void CastAbility2ServerRpc(Quaternion rot)
    {
        GameObject go = Instantiate(ability2Projectile, shootTransform.position, rot);
        go.GetComponent<NetworkObject>().Spawn();
    }

    private void Ability3Input()
    {
        if (Input.GetKeyDown(ability3Key) && !isAbility3Cooldown)
        {
            ability1Canvas.enabled = false;
            ability1Indicator.enabled = false;

            ability2Canvas.enabled = false;
            ability2Indicator.enabled = false;

            ability4Canvas.enabled = false;
            ability4Indicator.enabled = false;

            isAbility3Cooldown = true;
            currentAbility3Cooldown = ability3Cooldown;

            CastAbility3ServerRpc();
        }
    }

    [ServerRpc]
    private void CastAbility3ServerRpc()
    {
        GameManager.Instance.Speed(gameObject, Mathf.Log(stats.Damage, 10), NATURAL_LANGUAGE_PROCESSING_SPEED_DURATION);
    }

    private void Ability4Input()
    {
        if (Input.GetKeyDown(ability4Key) && !isAbility4Cooldown)
        {
            ability4Canvas.enabled = true;
            ability4Indicator.enabled = true;

            ability1Canvas.enabled = false;
            ability1Indicator.enabled = false;

            ability2Canvas.enabled = false;
            ability2Indicator.enabled = false;

            for (int i = 0; i < (int)stats.Damage / 20; i++)
            {
                CastAbility4ServerRpc();
            }

            isAbility4Cooldown = true;
            currentAbility4Cooldown = ability4Cooldown;

            ability4Canvas.enabled = false;
            ability4Indicator.enabled = false;

            GetComponent<OwnerNetworkAnimator>().SetTrigger("CastNeuralNetwork");
        }
    }

    [ServerRpc]
    private void CastAbility4ServerRpc()
    {

        GameObject go = Instantiate(ability4Projectile, new Vector3(shootTransform.position.x + UnityEngine.Random.Range(-5f, 5f), 
            shootTransform.position.y, shootTransform.position.z + UnityEngine.Random.Range(-5f, 5f)), UnityEngine.Random.rotation);
        Physics.IgnoreCollision(go.GetComponent<Collider>(), GetComponent<Collider>());
        go.GetComponent<MoveNeuralNetworkNode>().parent = this;
        go.GetComponent<NetworkObject>().Spawn();
    }

    private void AbilityCooldown(ref float currentCooldown, float maxCooldown, ref bool isCooldown, Image skillImage, TMP_Text skillText)
    {
        if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0f)
            {
                isCooldown = false;
                currentCooldown = 0f;

                if (skillImage != null)
                {
                    skillImage.fillAmount = 0f;
                }
                if (skillText != null)
                {
                    skillText.text = "";
                }
            }
            else
            {
                if (skillImage != null)
                {
                    skillImage.fillAmount = currentCooldown / maxCooldown;
                }
                if (skillText != null)
                {
                    skillText.text = Mathf.Ceil(currentCooldown).ToString();
                }
            }
        }
    }
}
