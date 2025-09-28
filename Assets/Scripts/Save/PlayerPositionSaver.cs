using UnityEngine;
using UnityEngine.SceneManagement;

namespace Taallam.Save
{
    [RequireComponent(typeof(Transform))]
    public class PlayerPositionSaver : MonoBehaviour
    {
        private void OnEnable()
        {
            ProfileManager.AfterLoad += ApplyLoadedPosition;
            ProfileManager.BeforeSave += CaptureCurrentPosition;
        }

        private void OnDisable()
        {
            ProfileManager.AfterLoad -= ApplyLoadedPosition;
            ProfileManager.BeforeSave -= CaptureCurrentPosition;
        }

        private void ApplyLoadedPosition(SaveFile file)
        {
            if (file?.player == null) return;
            transform.position = new Vector3(file.player.x, file.player.y, transform.position.z);
        }

        private void CaptureCurrentPosition(SaveFile file)
        {
            var pos = transform.position;
            file.player.scene = SceneManager.GetActiveScene().name;
            file.player.x = pos.x;
            file.player.y = pos.y;
        }
    }
}
