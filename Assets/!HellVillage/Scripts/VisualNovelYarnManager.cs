using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;
using Yarn.Unity;

namespace HellVillage.DialogueSystem {
    public class VisualNovelYarnManager : DialoguePresenterBase {
        [SerializeField] DialogueRunner dialogueRunner;
        [SerializeField] PlayableDirector timelineDirector;

        [Header("Assets"), Tooltip("Sprites to be used in the dialogue (Without using Resources folder)")]
        public List<Sprite> loadSprites = new List<Sprite>();
        public List<AudioClip> loadAudio = new List<AudioClip>();

        [Header("Sprite UI Settings")]
        [Tooltip("all sprites will be tinted with this color")]
        public Color defaultTint = new Color(184, 202, 216, 255) / 255f;
        [Tooltip("when speaking, a sprite will be highlighted by tinting it with this color")]
        public Color highlightTint = Color.white;

        [Header("Object references")]
        public Image nameplateBG;
        public Image genericSprite; // local prefab, used for instantiating sprites
        public AudioSource genericAudioSource; // local prefab, used for instantiating sounds
        public Canvas UICanvas;

        List<Image> sprites = new List<Image>();
        List<AudioSource> sounds = new List<AudioSource>();

        [HideInInspector]
        public Dictionary<string, VNYActor> actors = new Dictionary<string, VNYActor>();

        static Vector2 screenSize = new Vector2(1920, 1080);

        private void Awake() {
            dialogueRunner.AddCommandHandler<bool>("ToggleCanvas", ToggleUICanvas);

            dialogueRunner.AddCommandHandler<string, string, string, string, string>("Act", SetActor);
            dialogueRunner.AddCommandHandler<string, string, string>("Draw", SetSpriteYarn);

            dialogueRunner.AddCommandHandler<string, string, string, float>("Move", MoveSprite);
            dialogueRunner.AddCommandHandler<string, string>("Flip", FlipSprite);
            dialogueRunner.AddCommandHandler<string, float>("Shake", ShakeSprite);

            dialogueRunner.AddCommandHandler<string>("Hide", HideSprite);
            dialogueRunner.AddCommandHandler("HideAll", HideAllSprites);

            dialogueRunner.AddCommandHandler<string, float, string>("PlayAudio", PlayAudio);
            dialogueRunner.AddCommandHandler<string>("StopAudio", StopAudio);
            dialogueRunner.AddCommandHandler("StopAudioAll", StopAudioAll);

            dialogueRunner.AddCommandHandler("Done", VNDone);

            if (timelineDirector != null) {
                dialogueRunner.AddCommandHandler("PauseTimeline", (string enabled) => {
                    if (enabled == "true" || enabled == "1") {
                        timelineDirector.playableGraph.GetRootPlayable(0).SetSpeed(0f);
                    } else {
                        timelineDirector.playableGraph.GetRootPlayable(0).SetSpeed(1f);
                    }
                });
            } else {
                Debug.LogWarning("VN Manager can't play timelines because the PlayableDirector is not set in the inspector");
            }
        }

        #region Yarn Commands

        public void ToggleUICanvas(bool enabled) {
            if (UICanvas != null) {
                UICanvas.enabled = enabled;
            } else {
                Debug.LogWarning("VN Manager can't toggle UI Canvas because it is not set in the inspector");
            }
        }

        public void SetActor(string actorName, string spriteName, string positionX = "", string positionY = "", string colorHex = "#ffffff") {
            Image newActor = SetSpriteUnity(spriteName, positionX, positionY);

            Color actorColor = Color.black;
            if (colorHex != string.Empty && ColorUtility.TryParseHtmlString(colorHex, out actorColor) == false) {
                Debug.LogErrorFormat(this, "VN Manager can't parse [{0}] as an HTML color (e.g. [#FFFFFF] or certain keywords like [white])", colorHex);
            }

            if (actors.ContainsKey(actorName)) {
                Vector2 newPosition = newActor.rectTransform.anchoredPosition;
                if (positionX == string.Empty && positionY == string.Empty) {
                    newPosition = actors[actorName].rectTransform.anchoredPosition;
                } else if (positionY == string.Empty) {
                    newPosition.y = actors[actorName].rectTransform.anchoredPosition.y;
                }

                if (colorHex == string.Empty) {
                    actorColor = actors[actorName].actorColor;
                }

                newActor.rectTransform.anchoredPosition = newPosition;

                Destroy(actors[actorName].gameObject);
                actors.Remove(actorName);
                actors.Remove(actorName);
            }

            actors.Add(actorName, new VNYActor(newActor, actorColor));
        }

        public void SetSpriteYarn(string spriteName, string positionX = "", string positionY = "") {
            SetSpriteUnity(spriteName, positionX, positionY);
        }

        public Image SetSpriteUnity(string spriteName, string positionX = "", string positionY = "") {
            Vector2 position = new Vector2(0.5f, 0.5f);

            if (positionX != string.Empty) {
                position.x = ConvertCoordinates(positionX);
            }

            if (positionY != string.Empty) {
                position.y = ConvertCoordinates(positionY);
            }

            return SetSpriteActual(spriteName, position);
        }

        public void MoveSprite(string actorOrSpriteName, string screenPosX = "0.5", string screenPosY = "0.5", float moveTime = 1) {
            Image image = FindActorOrSprite(actorOrSpriteName);

            Vector2 newPosition = new Vector2(0.5f, 0.5f);
            if (screenPosX != string.Empty && screenPosY != string.Empty) {
                newPosition = new Vector2(ConvertCoordinates(screenPosX), ConvertCoordinates(screenPosY));
            } else if (screenPosX != string.Empty) {
                newPosition.x = ConvertCoordinates(screenPosX);
            }

            // actually do the moving now
            StartCoroutine(MoveCoroutine(image.GetComponent<RectTransform>(), Vector2.Scale(newPosition, screenSize), moveTime));
        }

        public void FlipSprite(string actorOrSpriteName, string xDirection = "") {
            Image image = FindActorOrSprite(actorOrSpriteName);

            float direction;

            if (xDirection != string.Empty) {
                direction = Mathf.Sign(ConvertCoordinates(xDirection) - 0.5f);
            } else {
                direction = Mathf.Sign(image.rectTransform.localScale.x) * -1f;
            }

            image.rectTransform.localScale = new Vector3(
                direction * Mathf.Abs(image.rectTransform.localScale.x),
                image.rectTransform.localScale.y,
                image.rectTransform.localScale.z
            );
        }

        public void ShakeSprite(string actorOrSpriteName, float shakeStrength = 0.5f) {

            Image findShakeTarget = FindActorOrSprite(actorOrSpriteName);
            if (findShakeTarget != null) {
                StartCoroutine(SetShake(findShakeTarget.rectTransform, shakeStrength));
            }
        }

        public void HideSprite(string spriteName) {
            Wildcard wildcard = new Wildcard(spriteName);

            var imagesToDestroy = new List<Image>();
            var actorKeysToRemove = new List<string>();

            foreach (KeyValuePair<string, VNYActor> actor in actors) {
                if (wildcard.IsMatch(actor.Key) || wildcard.IsMatch(actor.Value.actorImage.name)) {
                    actorKeysToRemove.Add(actor.Key);
                    imagesToDestroy.Add(actor.Value.actorImage);
                }
            }

            foreach (Image sprite in sprites) {
                if (wildcard.IsMatch(sprite.name)) {
                    imagesToDestroy.Add(sprite);
                }
            }

            for (int i = 0; i < actorKeysToRemove.Count; i++) {
                if (actors.ContainsKey(actorKeysToRemove[i])) {
                    actors.Remove(actorKeysToRemove[i]);
                }
            }

            for (int i = 0; i < imagesToDestroy.Count; i++) {
                if (imagesToDestroy[i] != null) {
                    CleanDestroy<Image>(imagesToDestroy[i].gameObject);
                }
            }
        }

        public void HideAllSprites() {
            HideSprite("*");
            actors.Clear();
            sprites.Clear();
        }

        public void PlayAudio(string soundName, float volume = 1, string loop = "") {

            AudioClip audioClip = FetchAsset<AudioClip>(soundName);
            // detect volume setting

            if (volume <= 0.01f) {
                Debug.LogWarningFormat(this, "VN Manager is playing sound {0} at very low volume ({1}), just so you know", soundName, volume);
            }

            // detect loop setting
            bool shouldLoop = loop.Contains("loop") || loop.Contains("true");

            // instantiate AudioSource and configure it (don't use
            // AudioSource.PlayOneShot because we also want the option to
            // use <<StopAudio>> and interrupt it)
            AudioSource newAudioSource = Instantiate<AudioSource>(genericAudioSource, genericAudioSource.transform.parent);
            newAudioSource.name = audioClip.name;
            newAudioSource.clip = audioClip;
            newAudioSource.volume *= volume;
            newAudioSource.loop = shouldLoop;
            newAudioSource.Play();
            sounds.Add(newAudioSource);

            // if it doesn't loop, let's set a max lifetime for this sound
            if (shouldLoop == false) {
                StartCoroutine(SetDestroyTime(newAudioSource, audioClip.length));
            }
        }

        public void StopAudio(string soundName) {

            // let's just do this in a sloppy way for now, and also assume
            // there's only one object like it
            AudioSource toDestroy = null;
            foreach (AudioSource audioObject in sounds) {
                if (audioObject.name == soundName) {
                    toDestroy = audioObject;
                    break;
                }
            }

            // double-check there's any audioSource to destroy tho, because
            // it might have been destroyed already
            if (toDestroy != null) {
                CleanDestroy<AudioSource>(toDestroy.gameObject);
            } else {
                Debug.LogWarningFormat(this, "VN Manager tried to <<StopAudio {0}>> but couldn't find any sound \"{0}\" currently playing. Double-check the name, or maybe it already stopped.", soundName);
            }
        }

        public void StopAudioAll() {
            List<AudioSource> toStop = new List<AudioSource>();
            foreach (AudioSource audioSrc in sounds) {
                toStop.Add(audioSrc);
            }
            foreach (AudioSource stopThis in toStop) {
                StopAudio(stopThis.name);
            }
        }

        public void VNDone() {
            HideAllSprites();
        }

        #endregion

        #region Utility Methods
        private Image SetSpriteActual(string spriteName, Vector2 position) {
            Image newSpriteObject = Instantiate<Image>(genericSprite, genericSprite.transform.parent);
            sprites.Add(newSpriteObject);
            newSpriteObject.name = spriteName;
            newSpriteObject.sprite = FetchAsset<Sprite>(spriteName);
            newSpriteObject.SetNativeSize();
            newSpriteObject.rectTransform.anchoredPosition = Vector2.Scale(position, screenSize);
            return newSpriteObject;
        }

        private Image FindActorOrSprite(string actorOrSpriteName) {
            if (actors.ContainsKey(actorOrSpriteName)) {
                return actors[actorOrSpriteName].actorImage;
            } else { // or is it a generic sprite?
                foreach (var sprite in sprites) { // lazy sprite name search
                    if (sprite.name == actorOrSpriteName) {
                        return sprite;
                    }
                }
                Debug.LogErrorFormat(this, "VN Manager couldn't find an actor or sprite with name \"{0}\", maybe it was misspelled or the sprite was hidden / destroyed already", actorOrSpriteName);
                return null;
            }
        }

        private float ConvertCoordinates(string coordinate) {
            if (actors.ContainsKey(coordinate)) {
                return actors[coordinate].rectTransform.anchoredPosition.x / screenSize.x;
            }

            string labelCoordinate = coordinate.ToLower().Replace(" ", "").Replace("_", "").Replace("-", "");
            switch (labelCoordinate) {
                case "leftedge":
                case "bottomedge":
                case "loweredge":
                    return 0f;
                case "left":
                case "bottom":
                case "lower":
                    return 0.25f;
                case "center":
                case "middle":
                    return 0.5f;
                case "right":
                case "top":
                case "upper":
                    return 0.75f;
                case "rightedge":
                case "topedge":
                case "upperedge":
                    return 1f;
                case "offleft":
                    return -0.33f;
                case "offright":
                    return 1.33f;
            }

            if (float.TryParse(coordinate, out float x)) {
                return x;
            } else {
                Debug.LogErrorFormat(this, "VN Yarn Manager couldn't convert position [{0}]... it must be an alignment (left, center, right, or top, middle, bottom) or a value (like 0.42 as 42%)", coordinate);
                return -1f;
            }
        }

        private IEnumerator MoveCoroutine(RectTransform transform, Vector2 newAnchorPosition, float moveTime) {
            Vector2 startPosition = transform.anchoredPosition;
            float t = 0f;

            while (t < 1f) {
                t += Time.deltaTime / Mathf.Max(0.001f, moveTime);
                transform.anchoredPosition = Vector2.Lerp(startPosition, newAnchorPosition, t);
                yield return 0;
            }
        }

        IEnumerator SetShake(RectTransform thingToShake, float shakeStrength = 0.5f) {
            var startPosition = thingToShake.anchoredPosition;

            while (shakeStrength > 0f) {
                shakeStrength -= Time.deltaTime;
                float shakeDistance = Mathf.Clamp(shakeStrength * 69f, 0f, 69f);
                float shakeFrequency = Mathf.Clamp(shakeStrength * 5f, 0f, 5f);
                thingToShake.anchoredPosition = startPosition + shakeDistance * new Vector2(Mathf.Sin(Time.time * shakeFrequency), Mathf.Sin(Time.time * shakeFrequency + 17f) * 0.62f);
                yield return 0;
            }

            thingToShake.anchoredPosition = startPosition;
        }

        IEnumerator SetDestroyTime(AudioSource destroyThis, float timeDelay) {
            float timer = timeDelay;
            while (timer > 0f) {
                if (destroyThis == null) { break; } // it could've been destroyed already, so let's just make sure
                if (destroyThis.isPlaying) {
                    timer -= Time.deltaTime;
                }
                yield return 0;
            }
            if (destroyThis != null) { // it could've been destroyed already, so let's just make sure
                CleanDestroy<AudioSource>(destroyThis.gameObject);
            }
        }

        private T FetchAsset<T>(string assetName) where T : Object {
            if (typeof(T) == typeof(Sprite)) {
                foreach (var spr in loadSprites) {
                    if (spr.name == assetName) {
                        return spr as T;
                    }
                }
            } else if (typeof(T) == typeof(AudioClip)) {
                foreach (var ac in loadAudio) {
                    if (ac.name == assetName) {
                        return ac as T;
                    }
                }
            }

            Debug.LogErrorFormat(this, "VN Manager can't find asset [{0}]... maybe it is misspelled, or isn't imported as {1}?", assetName, typeof(T).ToString());
            return null;
        }

        private void CleanDestroy<T>(GameObject destroyThis) {
            if (destroyThis == null) return;

            var component = destroyThis.GetComponent<T>();
            if (component == null) return;

            if (typeof(T) == typeof(Image)) {
                sprites.Remove(component as Image);
            } else if (typeof(T) == typeof(AudioSource)) {
                sounds.Remove(component as AudioSource);
            }

            Destroy(destroyThis);
        }

        public override YarnTask RunLineAsync(LocalizedLine line, LineCancellationToken token) {
            string actorName = line.CharacterName;

            if (string.IsNullOrEmpty(actorName) == false && actors.ContainsKey(actorName)) {
                HighlightSprite(actors[actorName].actorImage);
                nameplateBG.color = actors[actorName].actorColor;
                nameplateBG.gameObject.SetActive(true);
            } else {
                nameplateBG.gameObject.SetActive(false);
            }

            return YarnTask.Yield();
        }

        public void HighlightSprite(Image sprite) {
            StopCoroutine(HighlightSpriteCoroutine(null));
            StartCoroutine(HighlightSpriteCoroutine(sprite));
        }

        IEnumerator HighlightSpriteCoroutine(Image highlightedSprite) {
            float t = 0f;
            // over time, gradually change sprites to be "normal" or
            // "highlighted"
            while (t < 1f) {
                t += Time.deltaTime / 2f;
                foreach (var spr in sprites) {
                    Vector3 regularScalePreserveXFlip = new Vector3(Mathf.Sign(spr.transform.localScale.x), 1f, 1f);
                    if (spr != highlightedSprite) { // set back to normal
                        spr.transform.localScale = Vector3.MoveTowards(spr.transform.localScale, regularScalePreserveXFlip, Time.deltaTime);
                        spr.color = Color.Lerp(spr.color, defaultTint, Time.deltaTime * 5f);
                    } else { // a little bit bigger / brighter
                        spr.transform.localScale = Vector3.MoveTowards(spr.transform.localScale, regularScalePreserveXFlip * 1.05f, Time.deltaTime);
                        spr.color = Color.Lerp(spr.color, highlightTint, Time.deltaTime * 5f);
                        spr.transform.SetAsLastSibling();
                    }
                }
                yield return 0;
            }
        }

        public override YarnTask<DialogueOption> RunOptionsAsync(DialogueOption[] dialogueOptions, CancellationToken cancellationToken) {
            return YarnTask<DialogueOption>.FromResult(dialogueOptions[0]); // just return the first option for now
        }

        public override YarnTask OnDialogueStartedAsync() {
            return YarnTask.Yield();
        }

        public override YarnTask OnDialogueCompleteAsync() {
            return YarnTask.Yield();
        }

        #endregion
    }

    [System.Serializable]
    public class VNYActor {
        public Image actorImage;
        public Color actorColor;
        public RectTransform rectTransform { get { return actorImage.rectTransform; } }
        public GameObject gameObject { get { return actorImage.gameObject; } }

        public VNYActor(Image actorImage, Color actorColor) {
            this.actorImage = actorImage;
            this.actorColor = actorColor;
        }
    }

    // from
    // https://www.codeproject.com/Articles/11556/Converting-Wildcards-to-Regexes
    // by Rei Miyasaka
    class Wildcard : Regex {
        public Wildcard(string pattern) : base(WildcardToRegex(pattern)) { }

        public Wildcard(string pattern, RegexOptions options) : base(WildcardToRegex(pattern), options) { }

        public static string WildcardToRegex(string pattern) {
            return "^" + Regex.Escape(pattern).Replace("\\*", ".*").Replace("\\?", ".") + "$";
        }
    }
}
