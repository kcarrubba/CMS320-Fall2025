using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;

namespace DinoRunner {
	[Serializable]
	public class PlayerController : MonoBehaviour {
		
		[Serializable]
		private struct ObstacleNameMapping {
			public string name;
			public int typeId;
		}

		private float moveSpeed = 4.0f;
		private float jumpHeight = 10.0f;

		private const float AnimationFrameDuration = 0.1f;
		private float spriteTimer = AnimationFrameDuration;
		private int actualSprite = 0;
		private Sprite[] standingAnimationFrames;
		private Sprite[] crouchingAnimationFrames;
		private Sprite[] currentAnimationFrames;

		[SerializeField]
		private string[] standingSpriteNames = new[] { "walk1", "walk2" };

		[SerializeField]
		private string[] crouchingSpriteNames = new[] { "Crouch", "Crouch2" };

		[SerializeField]
		private float groundY = 0.09f;

		[SerializeField]
		private bool lockGroundY = true;

		[SerializeField]
		private ObstacleNameMapping[] obstacleNameMappings = new ObstacleNameMapping[] {
			new ObstacleNameMapping { name = "Boxes", typeId = 1 },
			new ObstacleNameMapping { name = "Laundry", typeId = 2 },
			new ObstacleNameMapping { name = "Cat", typeId = 3 }
		};

		private SpriteRenderer spriteRenderer;
		private Rigidbody2D rb;

		private String genomeBasePath = "Genomes/genome_";

		[SerializeField]
		private List<Cactus> cactus = new List<Cactus> ();

		[SerializeField]
		private List<Jumped> jumps = new List<Jumped>();

		private List<Genome> genomes;

		private bool isGrounded = true;
		private bool isCrouching = false;

		private int actualJumpGenome = 0;

		private bool isLearning = true; //If a real player is playing the game

		// Use this for initialization
		void Start () {	
			rb = GetComponent<Rigidbody2D>();
			if (rb == null) {
				Debug.LogError("No Rigidbody2D on the Player object.");
			}

			spriteRenderer = GetComponent<SpriteRenderer>();
			if (spriteRenderer == null) {
				Debug.LogError("No SpriteRenderer on the Player object.");
			} else if (spriteRenderer.color.a < 0.99f) {
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f); // ensure not transparent
			}

			if (lockGroundY) {
				SnapToGroundY();
			}

			genomes = Utils.loadAllGenomes();
			// DEBUG: make it obvious why dino is invisible
			standingAnimationFrames = LoadAnimationFrames("Art/Player/Standing", standingSpriteNames);
			crouchingAnimationFrames = LoadAnimationFrames("Art/Player/Crouching", crouchingSpriteNames);
			currentAnimationFrames = standingAnimationFrames;

			if ((standingAnimationFrames == null || standingAnimationFrames.Length == 0) && spriteRenderer != null) {
				Debug.LogError("Standing sprites not found at Resources/Art/Player/Standing. Player will stay invisible.");
			}
			if (crouchingAnimationFrames == null || crouchingAnimationFrames.Length == 0) {
				Debug.LogError("Crouching sprites not found at Resources/Art/Player/Crouching.");
			}
			if (currentAnimationFrames != null && currentAnimationFrames.Length > 0 && spriteRenderer != null) {
				spriteRenderer.sprite = currentAnimationFrames[0];
			}

			GetComponent<BoxCollider2D> ().enabled = false;

			isLearning = true;   // always manual control for now
			if (!isLearning && Utils.actualGenome >= genomes.Count) {
				print ("Acabou de jogar os genomas");
				Utils.clearCrossOversFolder();
				genomes = Utils.loadAllGenomes (); //Force GENOMES root folder

				List<Genome> bestGenomes = Utils.naturalSelection (genomes, 4);
				Utils.clearGenomesFolder();

				for (int i = 0; i < bestGenomes.Count; i++) {
					Utils.persistInJson (bestGenomes[i], genomeBasePath + i + "_");
				}
					
				//New Crossovers + Mutations
				for (int i = 0; i < 4; i++) {
					for (int j = 0; j < 4; j++) {
						if (i == j) {
							continue;
						}						

						Genome g = Genetic.crossOver (bestGenomes [i], bestGenomes [j]);
						g = Genetic.mutate (g);
						Utils.persistInJson (g, "Genomes/CrossedOvers/genome_" + i + "_" + j + "_");
					}
				}

				genomes = Utils.loadAllGenomes ();
				Utils.actualGenome = 0;
			}
			GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = false;

			//Load Cactus
			foreach(GameObject c in GameObject.FindGameObjectsWithTag("cactus"))
			{			
				string normalizedName = NormalizeObstacleName(c.name);
				int cacType = ResolveObstacleType(normalizedName);

				Cactus toAdd = new Cactus () {
					type = cacType,
					position = c.transform.position
				};
				cactus.Add (toAdd);
			}
		}
		
		// Update is called once per frame
		void Update () {		
			if (rb == null) {
				rb = GetComponent<Rigidbody2D>();
			}

			UpdateAnimationState();

			if (rb != null) {
				rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
				if (lockGroundY && isGrounded) {
					SnapToGroundY();
				}
			}

			if (isLearning) {
				if (Input.GetKeyUp (KeyCode.DownArrow)) {
					isCrouching = false;
					GetComponent<PolygonCollider2D> ().enabled = true;
					GetComponent<BoxCollider2D> ().enabled = false;
				}
					
				if (isGrounded && (Input.GetKeyDown (KeyCode.Space) || Input.GetKeyDown (KeyCode.UpArrow))) {
					isCrouching = false;
					GetComponent<PolygonCollider2D> ().enabled = true;
					GetComponent<BoxCollider2D> ().enabled = false;
					
					if (rb != null) {
						rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
					}

					isGrounded = false;

					Cactus c = getNextNearestCactus ();
					if (c != null) {
						Jumped jump = new Jumped {
							nearestCactus = c,
							distanceToNearestCactus = c.position - (rb != null ? rb.position : Vector2.zero)
						};

						jumps.Add (jump);
					}
				} else if (isGrounded && !isCrouching && Input.GetKeyDown (KeyCode.DownArrow)) {
					isCrouching = true;
					GetComponent<BoxCollider2D> ().enabled = true;
					GetComponent<PolygonCollider2D> ().enabled = false;
				}
			} else {
				if(Time.timeScale != 0) {
					if (Utils.actualGenome < genomes.Count) {
						playGenome(Utils.actualGenome);	
					}					
				}
			}
		}

		private void UpdateAnimationState() {
			if (spriteRenderer == null) {
				return;
			}

			Sprite[] targetAnimation = isCrouching ? crouchingAnimationFrames : standingAnimationFrames;
			if (targetAnimation == null || targetAnimation.Length == 0) {
				return;
			}

			if (!ReferenceEquals(targetAnimation, currentAnimationFrames)) {
				currentAnimationFrames = targetAnimation;
				actualSprite = 0;
				spriteRenderer.sprite = currentAnimationFrames[actualSprite];
				spriteTimer = AnimationFrameDuration;
			}

			spriteTimer -= Time.deltaTime;
			if (spriteTimer <= 0f && currentAnimationFrames.Length > 0) {
				spriteTimer = AnimationFrameDuration;
				actualSprite = (actualSprite + 1) % currentAnimationFrames.Length;
				spriteRenderer.sprite = currentAnimationFrames[actualSprite];
			}
		}

		// Called when a collision happens
		void OnCollisionEnter2D(Collision2D coll) {
			if (coll.gameObject.CompareTag("cactus")) {			
				GameObject.Find ("Canvas").GetComponent<Canvas> ().enabled = true;
				Time.timeScale = 0;

				Genome genome = new Genome {
					fitness = Genetic.calculateFitness(jumps, cactus),
					jumps = jumps
				};

				Utils.persistInJson (genome, genomeBasePath);

				jumps.Clear();

				if(!isLearning) {
					SceneManager.LoadScene(SceneManager.GetActiveScene().name);
					Time.timeScale = 1;
					Utils.actualGenome++;
				}				
			} else if (coll.gameObject.name.StartsWith ("Ground")) {
				isGrounded = true;
				if (lockGroundY) {
					SnapToGroundY();
				}
			}
		}

		private void SnapToGroundY() {
			if (rb != null) {
				rb.position = new Vector2(rb.position.x, groundY);
				transform.position = rb.position;
			} else {
				Vector2 pos = transform.position;
				pos.y = groundY;
				transform.position = pos;
			}
		}

		private string NormalizeObstacleName(string objectName) {
			if (string.IsNullOrEmpty(objectName)) {
				return string.Empty;
			}

			const string cloneSuffix = "(Clone)";
			int cloneIndex = objectName.IndexOf(cloneSuffix, StringComparison.Ordinal);
			if (cloneIndex >= 0) {
				objectName = objectName.Substring(0, cloneIndex);
			}

			return objectName.Trim();
		}

		private int ResolveObstacleType(string normalizedName) {
			if (obstacleNameMappings != null) {
				foreach (ObstacleNameMapping mapping in obstacleNameMappings) {
					if (!string.IsNullOrEmpty(mapping.name) && string.Equals(mapping.name.Trim(), normalizedName, StringComparison.OrdinalIgnoreCase)) {
						return mapping.typeId <= 0 ? 1 : mapping.typeId;
					}
				}
			}

			return 1;
		}

		private Sprite[] LoadAnimationFrames(string resourcePath, string[] preferredNames) {
			List<Sprite> frames = new List<Sprite>();

			if (preferredNames != null && preferredNames.Length > 0) {
				foreach (string preferredName in preferredNames) {
					string trimmedName = preferredName != null ? preferredName.Trim() : string.Empty;
					if (string.IsNullOrEmpty(trimmedName)) {
						continue;
					}

					Sprite sprite = Resources.Load<Sprite> ($"{resourcePath}/{trimmedName}");
					if (sprite != null) {
						frames.Add(sprite);
					} else {
						Debug.LogWarning($"Sprite '{trimmedName}' not found at Resources/{resourcePath}.");
					}
				}
			}

			if (frames.Count == 0) {
				Sprite[] fallback = Resources.LoadAll<Sprite>(resourcePath);
				if (fallback != null && fallback.Length > 0) {
					Array.Sort(fallback, (left, right) => string.CompareOrdinal(
						left != null ? left.name : string.Empty,
						right != null ? right.name : string.Empty));

					foreach (Sprite sprite in fallback) {
						if (sprite != null) {
							frames.Add(sprite);
						}
					}
				}
			}

			return frames.ToArray();
		}

		Cactus getNextNearestCactus() {
			float nearestDist = float.PositiveInfinity;
			Cactus nearestCactus = null;
			foreach(Cactus c in cactus)
			{
				float cacX = c.position.x;
				float playerX = rb != null ? rb.position.x : 0f;
				if (cacX > playerX) {
					float dist = cacX - playerX;
					if (dist < nearestDist) {
						nearestDist = dist;
						nearestCactus = c;
					}
				}
			}

			return nearestCactus;
		}

		void playGenome(int genomeIndex) {
			if (rb == null) {
				rb = GetComponent<Rigidbody2D>();
				if (rb == null) {
					return;
				}
			}

			Cactus nextCactus = getNextNearestCactus();
			if (nextCactus == null) {
				return;
			}

			float dist = nextCactus.position.x - rb.position.x;
			if (actualJumpGenome >= genomes[genomeIndex].jumps.Count) {
			}
			else if (dist <= genomes[genomeIndex].jumps[actualJumpGenome].distanceToNearestCactus.x && isGrounded) {
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);

				isGrounded = false;

				Cactus c = nextCactus;
				Jumped jump = new Jumped {
					nearestCactus = c,
					distanceToNearestCactus = c.position - rb.position
				};

				jumps.Add (jump);

				actualJumpGenome++;
			}
		}
	}
}
