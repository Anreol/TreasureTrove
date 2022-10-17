using RoR2;
using UnityEngine;

namespace TreasureTrove.Components
{
	[RequireComponent(typeof(SpriteRenderer))]
	public class SimpleSpriteRendererAnimator : MonoBehaviour
	{
		private void Awake()
		{
			this.target = base.GetComponent<SpriteRenderer>();
		}

		private void Update()
		{
			if (!this.animation)
			{
				return;
			}
			if (this.animation.frames.Length == 0)
			{
				return;
			}
			this.tickStopwatch += Time.fixedDeltaTime;
			float num = 1f / this.animation.frameRate;
			if (this.tickStopwatch > num)
			{
				this.tickStopwatch -= num;
				this.Tick();
			}
		}

		private void Tick()
		{
			this.tick++;
			if (this.frame >= this.animation.frames.Length)
			{
                if (destroySelfOnFinish)
                {
					GameObject.Destroy(this);
					return;
                }
				this.frame = 0;
				this.tick = 0;
			}
			ref SimpleSpriteAnimation.Frame ptr = ref this.animation.frames[this.frame];
			if (ptr.duration <= this.tick)
			{
				this.frame++;
				if (this.frame >= this.animation.frames.Length)
				{
					this.frame = 0;
					this.tick = 0;
				}
				ptr = ref this.animation.frames[this.frame];
				this.target.sprite = ptr.sprite;
			}
		}

		public SimpleSpriteAnimation animation;
		[Tooltip("Should the component destroy its gameobject when reaching the final frame.")]
		public bool destroySelfOnFinish;
		private SpriteRenderer target;
		private int frame;
		private int tick;
		private float tickStopwatch;
	}
}
