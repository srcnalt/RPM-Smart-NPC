using Cinemachine;
using StarterAssets;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public static class ThirdPersonCharacterBuilder
{
    private const string ANIMATOR_PATH = "Assets/StarterAssets/ThirdPersonController/Character/Animations/StarterAssetsThirdPerson.controller";
    private const string INPUT_ASSET_PATH = "Assets/StarterAssets/InputSystem/StarterAssets.inputactions";
    private const string CAMERA_TARGET_OBJECT_NAME = "CameraTarget";

    private const string LANDING_AUDIO_PATH = "Assets/StarterAssets/ThirdPersonController/Character/Sfx/Player_Land.wav";
    private const string FOOTSTEP_AUDIO_PATH = "Assets/StarterAssets/ThirdPersonController/Character/Sfx/Player_Footstep";

    [MenuItem("Ready Player Me/Setup Character", true, 0)]
    public static bool SetupCharacterValidate()
    {
        return Selection.activeGameObject != null;
    }

    [MenuItem("Ready Player Me/Setup Character")]
    public static void SetupCharacter()
    {
        // Cache selected object to add the components
        GameObject character = Selection.activeGameObject;

        character.tag = "Player";

        // Create camera follow target
        GameObject cameraTarget = new GameObject(CAMERA_TARGET_OBJECT_NAME);
        cameraTarget.transform.parent = character.transform;
        cameraTarget.transform.localPosition = new Vector3(0, 1.5f, 0);
        cameraTarget.tag = "CinemachineTarget";

        // Set the animator controller and disable root motion
        Animator animator = character.GetComponent<Animator>();
        animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(ANIMATOR_PATH);
        animator.applyRootMotion = false;

        // Add tp controller and set values
        ThirdPersonController tpsController = character.AddComponent<ThirdPersonController>();
        tpsController.GroundedOffset = 0.1f;
        tpsController.GroundLayers = 1;
        tpsController.JumpTimeout = 0.5f;
        tpsController.CinemachineCameraTarget = cameraTarget;
        tpsController.LandingAudioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(LANDING_AUDIO_PATH);
        tpsController.FootstepAudioClips = new AudioClip[]
        {
            AssetDatabase.LoadAssetAtPath<AudioClip>($"{FOOTSTEP_AUDIO_PATH}_01.wav"),
            AssetDatabase.LoadAssetAtPath<AudioClip>($"{FOOTSTEP_AUDIO_PATH}_02.wav")
        };
        
        // Add character controller and set size
        CharacterController characterController = character.GetComponent<CharacterController>();
        characterController.center = new Vector3(0, 1, 0);
        characterController.radius = 0.3f;
        characterController.height = 1.9f;

        // Add components with default values
        character.AddComponent<BasicRigidBodyPush>();
        character.AddComponent<StarterAssetsInputs>();
    
        // Add player input and set actions asset
        PlayerInput playerInput = character.GetComponent<PlayerInput>();
        playerInput.actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>(INPUT_ASSET_PATH);
    
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        
        // 
        var camera = Object.FindObjectOfType<CinemachineVirtualCamera>();
        if (camera)
        {
            camera.Follow = cameraTarget.transform;
            camera.LookAt = cameraTarget.transform;
        }
    }
}