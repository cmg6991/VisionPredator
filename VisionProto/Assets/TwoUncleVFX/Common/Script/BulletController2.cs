using System.Threading.Tasks;
using UnityEngine;

namespace VFXTools
{
	public class BulletController : MonoBehaviour
	{
		public float rotationSpeed = 100f; 
		public float movementSpeed = 10f;
		public float delayTime = 0f;
		private bool isPlay = false;
		public float time = 1f;
		private float lastTime = 0f;
		private Vector3 startPos;

		private void Start()
		{
			startPos = transform.position;
			SetPlay(true);
		}

		private async void SetPlay(bool play)
		{
			await Task.Delay((int)(delayTime * 1000));
			isPlay = play;
		}
		private void Update()
		{
			if(!isPlay) return;
			lastTime += Time.deltaTime;
			if (lastTime > time)
			{
				transform.position = startPos;
				lastTime = 0f;
				return;
			}
			Vector3 directionToCenter = transform.forward;
			Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
		}
	}
}