using UnityEngine;

public class ThirdPersonToolAttachment : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private Transform thirdPersonToolHolder;

    private void Start()
    {
        AttachToRightHand();
    }

    private void AttachToRightHand()
    {
        if (characterAnimator == null)
        {
            Debug.LogWarning(
                "ThirdPersonToolAttachment: Kein Animator zugewiesen.",
                this
            );

            return;
        }

        if (thirdPersonToolHolder == null)
        {
            Debug.LogWarning(
                "ThirdPersonToolAttachment: Kein ThirdPersonToolHolder zugewiesen.",
                this
            );

            return;
        }

        Transform rightHand =
            characterAnimator.GetBoneTransform(
                HumanBodyBones.RightHand
            );

        if (rightHand == null)
        {
            Debug.LogWarning(
                "ThirdPersonToolAttachment: RightHand-Bone wurde nicht gefunden. " +
                "Prüfe, ob der Avatar als Humanoid konfiguriert ist.",
                this
            );

            return;
        }

        thirdPersonToolHolder.SetParent(
            rightHand,
            false
        );

        thirdPersonToolHolder.localPosition =
            Vector3.zero;

        thirdPersonToolHolder.localRotation =
            Quaternion.identity;

        thirdPersonToolHolder.localScale =
            Vector3.one;
    }
}