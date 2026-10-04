using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
namespace Farmer
{
    public static class Common
    {
        public static Coroutine SetTimeout(this MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            return monoBehaviour.StartCoroutine(TimeoutScaledTime(delay, callback));
        }

        public static Coroutine SetTimeoutScaledTime(this MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            return monoBehaviour.StartCoroutine(TimeoutScaledTime(delay, callback));
        }

        public static Coroutine SetTimeoutUnscaledTime(this MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            return monoBehaviour.StartCoroutine(TimeoutUnscaledTime(delay, callback));
        }

        public static Coroutine SetTimeout(this MonoBehaviour monoBehaviour, float delay, Action callback)
        {
            return monoBehaviour.StartCoroutine(Timeout(delay, callback));
        }

        public static Coroutine WaitNextFrame(this MonoBehaviour monoBehaviour, Action callback)
        {
            return monoBehaviour.StartCoroutine(WaitNextFrame(callback));
        }

        public static Coroutine WaitSomeFrames(this MonoBehaviour monoBehaviour, int frames, Action callback)
        {
            return monoBehaviour.StartCoroutine(WaitSomeFrames(frames, callback));
        }

        public static Coroutine WaitUntil(this MonoBehaviour monoBehaviour, Func<bool> predicate, Action callback)
        {
            return monoBehaviour.StartCoroutine(WaitUntil(predicate, callback));
        }

        public static Coroutine WaitUntilWithTimeout(this MonoBehaviour monoBehaviour, Func<bool> predicate, Action callback, float maxWaitTime = 10)
        {
            return monoBehaviour.StartCoroutine(WaitUntilWithTimeout(predicate, callback, maxWaitTime));
        }

        //public static Coroutine WaitUntilWithTimer(this MonoBehaviour monoBehaviour, Func<bool> predicate, Action callback)
        //{
        //    return monoBehaviour.StartCoroutine(WaitUntil(predicate, callback));

        //}

        public static Coroutine CheckInternetConnection(this MonoBehaviour monoBehaviour, Action<bool> action)
        {
            return monoBehaviour.StartCoroutine(CheckInternetConnection(action));
        }

        private static IEnumerator Timeout(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }

        private static IEnumerator TimeoutScaledTime(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }

        private static IEnumerator TimeoutUnscaledTime(float delay, Action callback)
        {
            yield return new WaitForSecondsRealtime(delay);
            callback();
        }
        private static IEnumerator CheckInternetConnection(Action<bool> action)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                action?.Invoke(false);
                yield break;
            }

            using var request = UnityWebRequest.Get("https://gaia-survivor.eztechglobal.com/api/client/info");
            request.timeout = 5;
            yield return request.SendWebRequest();
            action?.Invoke(request.result == UnityWebRequest.Result.Success);
        }

        public static Coroutine SetInterval(this MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            return monoBehaviour.StartCoroutine(Interval(delay, callback));
        }

        public static Coroutine SetIntervalSafely(this MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            return monoBehaviour.StartCoroutine(IntervalSafely(delay, callback));
        }

        public static Coroutine SetRandomInterval(this MonoBehaviour monoBehaviour, Action callback, float delay, float deviation = 0)
        {
            return monoBehaviour.StartCoroutine(RandomInterval(delay, callback, deviation));
        }

        public static Coroutine SetIntervalCallFirst(this MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            return monoBehaviour.StartCoroutine(IntervalCallFirst(delay, callback));
        }

        public static Coroutine SetIntervalUnscaledTime(this MonoBehaviour monoBehaviour, Action callback, float delay)
        {
            return monoBehaviour.StartCoroutine(IntervalUnscaledTime(delay, callback));
        }
        public static Coroutine SetIntervalFrame(this MonoBehaviour monoBehaviour, Action callback, int frameCount)
        {
            return monoBehaviour.StartCoroutine(IntervalFrame(frameCount, callback));
        }
        public static Coroutine SetIntervalFrameCallFirst(this MonoBehaviour monoBehaviour, Action callback, int frameCount)
        {
            return monoBehaviour.StartCoroutine(IntervalFrameCallFirst(frameCount, callback));
        }
        public static Coroutine LateFixedUpdate(this MonoBehaviour monoBehaviour, Action callback)
        {
            return monoBehaviour.StartCoroutine(LateFixedUpdate(callback));
        }

        private static IEnumerator Interval(float delay, Action callback)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                callback();
            }
        }

        private static IEnumerator IntervalSafely(float delay, Action callback)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(delay);
                try
                {
                    callback();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

            }
        }

        private static IEnumerator RandomInterval(float delay, Action callback, float deviation = 0)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay + UnityEngine.Random.Range(-deviation, deviation));
                callback();
            }
        }

        private static IEnumerator IntervalCallFirst(float delay, Action callback)
        {
            while (true)
            {
                callback();
                yield return new WaitForSeconds(delay);
            }
        }

        private static IEnumerator IntervalUnscaledTime(float delay, Action callback)
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(delay);
                callback();
            }
        }
        private static IEnumerator IntervalFrame(int frameCount, Action callback)
        {
            while (true)
            {
                for (int i = 0; i < frameCount; i++)
                {
                    yield return null;
                }
                callback();
            }
        }
        private static IEnumerator IntervalFrameCallFirst(int frameCount, Action callback)
        {
            while (true)
            {
                callback();
                for (int i = 0; i < frameCount; i++)
                {
                    yield return null;
                }
            }
        }

        private static IEnumerator WaitNextFrame(Action callback)
        {
            yield return new WaitForEndOfFrame();
            callback();
        }

        private static IEnumerator WaitSomeFrames(int frames, Action callback)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            callback();
        }

        private static IEnumerator WaitUntil(Func<bool> predicate, Action callback)
        {
            yield return new WaitUntil(predicate);
            callback();
        }

        private static IEnumerator WaitUntilWithTimeout(Func<bool> predicate, Action callback, float maxWaitTime = 10)
        {
            float timeoutTime = Time.time + maxWaitTime;
            while (!predicate() && Time.time < timeoutTime)
            {
                yield return null;
            }
            if (predicate())
                callback();
        }

        private static IEnumerator LateFixedUpdate(Action callback)
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                callback();
            }
        }

        public static Coroutine TransformMoveTo(this MonoBehaviour monoBehaviour, Transform source, Transform target, float duration)
        {
            return monoBehaviour.StartCoroutine(TransformMoveTo(source, target, duration));
        }
        public static IEnumerator TransformMoveTo(this Transform source, Transform target, float duration)
        {
            float timer = 0;
            source.SetParent(target);
            yield return null;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                source.position = Vector3.Lerp(source.position, target.position, timer / duration);
                // source.rotation = Quaternion.Slerp(source.rotation, target.rotation, timer / duration);
                yield return null;
            }
            source.position = target.position;
            // source.rotation = target.rotation;
        }
        public static void DestroyAllChildren(this Transform transform)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(transform.GetChild(i).gameObject);
            }
        }

        public static void AddListenerOnce<T>(this UnityEvent<T> events, UnityAction<T> callback)
        {
            events.RemoveListener(callback);
            events.AddListener(callback);
        }

        public static void AddListenerOnce(this UnityEvent events, UnityAction callback)
        {
            events.RemoveListener(callback);
            events.AddListener(callback);
        }

        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");

            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            byte a = 255;

            if (hex.Length == 8) // Có alpha
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);

            return new Color32(r, g, b, a);
        }
        public static Color SetAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        public static bool Vector3Approximately(Vector3 a, Vector3 b, float tolerance = 0.0001f)
        {
            return (a - b).sqrMagnitude < tolerance * tolerance;
        }
        public static Transform GetLastChildren(this Transform transform)
        {
            if (transform.childCount == 0) return null;
            return transform.GetChild(transform.childCount - 1);
        }
        public static Vector3 GetWorldPosition(this RectTransform rt)
        {
            Vector3[] corners = new Vector3[4];
            rt.GetWorldCorners(corners);
            return (corners[0] + corners[2]) * 0.5f;   // center position
        }
        public static void CopyRectTransformWorld(RectTransform source, RectTransform target)
        {
            // Copy size
            target.sizeDelta = source.sizeDelta;
            target.pivot = source.pivot;

            // Copy rotation + scale
            target.rotation = source.rotation;
            target.localScale = source.localScale;

            // Copy world position
            target.position = source.position;
        }
        public static void CopyRectTransform_KeepScreenPosition(RectTransform src, RectTransform dst)
        {
            // 1. Copy world position
            dst.position = src.position;

            // 2. Copy rotation (optional)
            dst.rotation = src.rotation;

            // 3. Copy world size từ src sang dst
            Vector3[] corners = new Vector3[4];
            src.GetWorldCorners(corners);

            float width = Vector3.Distance(corners[0], corners[3]);
            float height = Vector3.Distance(corners[0], corners[1]);

            dst.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            dst.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            // 4. Copy pivot (để match hình)
            dst.pivot = src.pivot;
        }
        public static void CopyUIOverlay(RectTransform src, RectTransform dst, Canvas canvas)
        {
            // 1. lấy screen pos chính xác của src
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, src.position);

            // 2. convert screen pos về local pos trong parent của dst
            RectTransform dstParent = dst.parent as RectTransform;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dstParent, screenPos, null, out localPoint);

            // 3. gán anchoredPosition → luôn đúng trên mọi độ phân giải
            dst.anchoredPosition = localPoint;

            // 4. copy kích thước (CanvasScaler sẽ scale tự động)
            dst.sizeDelta = src.sizeDelta;

            // 5. copy pivot nếu cần
            dst.pivot = src.pivot;
        }
        public static void CopyUIRectBetweenParents(RectTransform src, RectTransform dst, Camera uiCamera = null)
        {
            //
            // STEP 1 — Lấy screen position của src (SCREEN SPACE)
            //
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(uiCamera, src.position);
            //
            // STEP 2 — Convert screenPos → localPoint trong parent của dst
            //
            RectTransform dstParent = dst.parent as RectTransform;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                dstParent,
                screenPos,
                null,           // null vì Overlay Canvas không cần camera
                out localPoint
            );
            //
            // STEP 3 — Gán anchoredPosition chính xác cho dst
            //
            dst.anchoredPosition = localPoint;

            //
            // STEP 4 — Copy sizeDelta 1:1 (CanvasScaler sẽ lo scale đúng)
            //
            dst.sizeDelta = src.sizeDelta;

            //
            // STEP 5 — (OPTIONAL) copy rotation, pivot
            //
            dst.pivot = src.pivot;
            dst.localRotation = src.localRotation;
            dst.position = src.position;
        }

        public static void CopyRectTransformExact(RectTransform src, RectTransform dst)
        {
            Vector3[] corners = new Vector3[4];
            src.GetWorldCorners(corners);

            // Tính width/height theo world
            float width = corners[2].x - corners[0].x;
            float height = corners[2].y - corners[0].y;

            // 1. Copy size
            dst.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            dst.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

            // 2. Convert screen point to dst parent
            Vector2 screenCenter = (RectTransformUtility.WorldToScreenPoint(null, corners[0]) +
                                    RectTransformUtility.WorldToScreenPoint(null, corners[2])) * 0.5f;

            RectTransform dstParent = dst.parent as RectTransform;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(dstParent, screenCenter, null, out localPoint);

            // 3. Set anchored position → TRÙNG 100%
            dst.anchoredPosition = localPoint;
        }
        public static bool NearlyEqual(float a, float b, float tolerance = 0.0001f)
        {
            return Mathf.Abs(a - b) < tolerance;
        }

        public static Texture2D ConvertToRGBA32(this Texture2D source)
        {
            RenderTexture rt = RenderTexture.GetTemporary(
                source.width,
                source.height,
                0,
                RenderTextureFormat.ARGB32,
                RenderTextureReadWrite.Linear
            );

            Graphics.Blit(source, rt);

            RenderTexture prev = RenderTexture.active;
            RenderTexture.active = rt;

            Texture2D readableTex = new Texture2D(
                source.width,
                source.height,
                TextureFormat.RGBA32,
                false
            );

            readableTex.ReadPixels(
                new Rect(0, 0, rt.width, rt.height),
                0, 0
            );
            readableTex.Apply();

            RenderTexture.active = prev;
            RenderTexture.ReleaseTemporary(rt);

            return readableTex;
        }
        public static Transform ContinousLookAt(this Transform source, MonoBehaviour monoBehaviour, Vector3 target, Vector3 upVector, float duration)
        {
            monoBehaviour.StartCoroutine(LookAtCoroutine(source, target, upVector, duration));
            return source;
        }
        private static IEnumerator LookAtCoroutine(Transform source, Vector3 target, Vector3 upVector, float duration)
        {
            float timer = 0;
            Quaternion initialRotation = source.rotation;
            source.LookAt(target, upVector);
            Quaternion targetRotation = source.rotation;
            source.rotation = initialRotation;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                source.rotation = Quaternion.Slerp(initialRotation, targetRotation, timer / duration);
                yield return null;
            }
            source.rotation = targetRotation;
        }

        public static List<T> ShuffleList<T>(this List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = list[n];
                list[n] = list[k];
                list[k] = temp;
            }
            return list;
        }
        public static T GetRandomItem<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default(T);
            int index = UnityEngine.Random.Range(0, list.Count);
            return list[index];
        }
        public static int ToMilliseconds(this float seconds)
        {
            return Mathf.RoundToInt(seconds * 1000);
        }
        public static bool TryToFind<T>(this List<T> list, Func<T, bool> predicate, out T result)
        {
            result = default(T);
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    result = item;
                    return true;
                }
            }
            return false;
        }
        // pool: list of (item, weight)
        public static T PickFromPool<T>(List<KeyValuePair<T, float>> pool)
        {
            if (pool == null || pool.Count == 0)
            {
                Debug.LogError("PickFromPool failed: pool is null or empty.");
                return default(T);
            }

            float totalWeight = 0f;
            T lastValidItem = default(T);
            bool hasValidItem = false;
            foreach (var item in pool)
            {
                if (item.Value <= 0f)
                {
                    Debug.LogError($"PickFromPool ignored item with invalid weight: {item.Value}");
                    continue;
                }

                totalWeight += item.Value;
                lastValidItem = item.Key;
                hasValidItem = true;
            }

            if (!hasValidItem || totalWeight <= 0f)
            {
                Debug.LogError("PickFromPool failed: pool has no item with positive weight.");
                return default(T);
            }

            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            float cumulativeWeight = 0f;

            foreach (var item in pool)
            {
                if (item.Value <= 0f)
                {
                    continue;
                }

                cumulativeWeight += item.Value;
                if (randomValue < cumulativeWeight)
                {
                    return item.Key;
                }
            }

            Debug.LogError("PickFromPool fallback reached. Check weight values for precision or invalid data.");
            return lastValidItem;
        }
        public static List<T> GetItemsExcept<T>(this List<T> source, List<T> excluded)
        {
            if (excluded == null || excluded.Count == 0) return new List<T>(source);
            return source.Where(item => !excluded.Contains(item)).ToList();
        }
        public static void SetSpriteWithAspectRatio(this Image img, Sprite sprite)
        {
            if (img.TryGetComponent<AspectRatioFitter>(out var fitter))
            {
                if (sprite != null)
                {
                    float aspectRatio = sprite.rect.width / sprite.rect.height;
                    fitter.aspectRatio = aspectRatio;
                }
            }
        }

        public static Vector2 WorldToCanvasPosition(Camera cam, Vector3 worldPosition, RectTransform rect)
        {
            if (cam == null)
            {
                Debug.Log("Cam is null");
                return Vector2.zero;
            }
            if (rect == null)
            {
                Debug.Log("Rect is null");
                return Vector2.zero;
            }
            Vector3 screenPosition =
                cam.WorldToScreenPoint(worldPosition);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect,
                screenPosition,
                null, // Canvas Screen Space - Overlay
                out Vector2 localPosition
            );
            return localPosition;
        }

        public static Vector2 ClampRectTransformPosition(
            RectTransform target,
            RectTransform bounds,
            Vector2 position,
            float padding = 8f)
        {
            if (target == null || bounds == null)
                return position;

            Rect targetRect = target.rect;
            Rect boundsRect = bounds.rect;
            float halfWidthOffset = targetRect.width * Mathf.Abs(target.localScale.x) * 0.5f;
            float halfHeightOffset = targetRect.height * Mathf.Abs(target.localScale.y) * 0.5f;
            float centerOffsetX = (0.5f - target.pivot.x) * targetRect.width * Mathf.Abs(target.localScale.x);
            float centerOffsetY = (0.5f - target.pivot.y) * targetRect.height * Mathf.Abs(target.localScale.y);

            float minX = boundsRect.xMin + padding + halfWidthOffset - centerOffsetX;
            float maxX = boundsRect.xMax - padding - halfWidthOffset - centerOffsetX;
            float minY = boundsRect.yMin + padding + halfHeightOffset - centerOffsetY;
            float maxY = boundsRect.yMax - padding - halfHeightOffset - centerOffsetY;

            position.x = minX <= maxX ? Mathf.Clamp(position.x, minX, maxX) : (minX + maxX) * 0.5f;
            position.y = minY <= maxY ? Mathf.Clamp(position.y, minY, maxY) : (minY + maxY) * 0.5f;
            return position;
        }
    }
    [Serializable]
    public partial class ValueKeyPair<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public ValueKeyPair() { }
        public ValueKeyPair(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }
    [Serializable]
    public partial class ValueKeyPair<TKey, T1, T2>
    {
        public TKey Key;
        public T1 V1;
        public T2 V2;
        public ValueKeyPair() { }
        public ValueKeyPair(TKey key, T1 v1, T2 v2)
        {
            Key = key;
            V1 = v1;
            V2 = v2;
        }
    }
    public static class CommonMath
    {
        public static Vector2 ToXZ(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector3 FromXZ(this Vector2 v, float y = 0)
        {
            return new Vector3(v.x, y, v.y);
        }
        public static float SmoothAngle(float from, float to)
        {
            float delta = Mathf.Repeat((to - from), 360f);
            if (delta > 180f) delta -= 360f;
            return from + delta;
        }
        public static float DistanceXZ(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dz = a.z - b.z;
            return Mathf.Sqrt(dx * dx + dz * dz);
        }

        public static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
        public static bool CompareEpsilon(float a, float b, float epsilon = 0.0001f)
        {
            return Mathf.Abs(a - b) < epsilon;
        }
        public static bool GetRandomBool(float trueProbability = 0.5f)
        {
            return UnityEngine.Random.value < trueProbability;
        }
    }
    public class CommonSequence
    {
        private class ActionItem
        {
            public float delay;
            public Action callback;

            public ActionItem(float delay, Action callback)
            {
                this.delay = delay;
                this.callback = callback;
            }
        }
        private List<ActionItem> sequence;

        public CommonSequence()
        {
            sequence = new List<ActionItem>();
        }
        public CommonSequence Add(float delay, Action action)
        {
            sequence.Add(new ActionItem(delay, action));
            return this;
        }

        public void StartSequence(MonoBehaviour monoBehaviour)
        {
            monoBehaviour.StartCoroutine(RunSequence());
        }

        private IEnumerator RunSequence()
        {
            foreach (var item in sequence)
            {
                yield return new WaitForSeconds(item.delay);
                item.callback?.Invoke();
            }
        }
    }

    public static class NavMeshUtil
    {
        private const float MinBuildingSampleRadius = 0.05f;
        private const float BuildingPointDedupeDistanceSqr = 0.04f * 0.04f;

        public struct BuildingNavMeshSamplingOptions
        {
            public float StandOffDistance;
            public float NavMeshSampleRadius;
            public int LateralSampleCount;
            public float LateralSampleSpacing;
            public int ForwardSampleCount;
            public float ForwardSampleSpacing;
            public float PathLengthWeight;
            public float IdealDistanceWeight;
            public float DirectionPriorityWeight;

            public static BuildingNavMeshSamplingOptions Default => new BuildingNavMeshSamplingOptions
            {
                StandOffDistance = 0.35f,
                NavMeshSampleRadius = 2f,
                LateralSampleCount = 2,
                LateralSampleSpacing = 0.4f,
                ForwardSampleCount = 1,
                ForwardSampleSpacing = 0.4f,
                PathLengthWeight = 1f,
                IdealDistanceWeight = 0.75f,
                DirectionPriorityWeight = 0.25f,
            };
        }

        public static bool TryToGetPathLengthAroundTarget(NavMeshAgent agent, Vector3 target, float avoidRadius, out float length)
        {
            length = 0f;
            if (agent == null) return false;
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, avoidRadius, NavMesh.AllAreas))
            {
                return TryToGetPathLength(agent, hit.position, out length);
            }
            length = float.PositiveInfinity;
            return false;
        }
        public static bool TryToGetPathLength(NavMeshAgent agent, Vector3 target, out float length)
        {
            NavMeshPath path = new NavMeshPath();
            if (!agent.CalculatePath(target, path))
            {
                length = float.PositiveInfinity;
                return false;
            }

            length = 0f;
            var corners = path.corners;
            for (int i = 1; i < corners.Length; i++)
            {
                length += Vector3.Distance(corners[i - 1], corners[i]);
            }
            return true;
        }
        public static bool TryToGetClosestNavMeshPoint(Collider targetCollider, float maxDistance, out Vector3 closestPoint)
        {
            Vector3 targetPoint = targetCollider.bounds.center;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPoint, out hit, maxDistance, NavMesh.AllAreas))
            {
                closestPoint = hit.position;
                return true;
            }

            closestPoint = targetPoint;
            return false;
        }
        public static bool TryToGetClosestPointToAgent(NavMeshAgent agent, Collider target, out Vector3 closestPoint)
        {
            Vector3 closest = target.ClosestPoint(agent.transform.position);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(closest, out hit, 1.5f, NavMesh.AllAreas))
            {
                closestPoint = hit.position;
                return true;
            }

            closestPoint = closest;
            return false;
        }
        public static bool TryToGetStopPoint(NavMeshAgent agent, Collider target, out Vector3 stopPoint)
        {
            Vector3 dir = (agent.transform.position - target.bounds.center).normalized;

            float distance = target.bounds.extents.magnitude + agent.radius;

            Vector3 point = target.bounds.center + dir * distance;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(point, out hit, 2f, NavMesh.AllAreas))
            {
                stopPoint = hit.position;
                return true;
            }

            stopPoint = point;
            return false;
        }
        public static bool TryToCalculatePathLength(NavMeshAgent agent, Vector3 target, out float length)
        {
            NavMeshPath path = new NavMeshPath();
            if (!agent.CalculatePath(target, path))
            {
                length = float.PositiveInfinity;
                return false;
            }
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                length = float.PositiveInfinity;
                return false;
            }
            length = 0f;
            var corners = path.corners;
            for (int i = 1; i < corners.Length; i++)
            {
                length += Vector3.Distance(corners[i - 1], corners[i]);
                Debug.DrawLine(corners[i - 1], corners[i], Color.red, 10f);
            }
            return true;
        }
        public static float CalculatePathLength(NavMeshPath path)
        {
            float length = 0f;
            var corners = path.corners;
            for (int i = 1; i < corners.Length; i++)
            {
                length += Vector3.Distance(corners[i - 1], corners[i]);
            }
            return length;
        }
        public static bool IsHaveReachablePath(NavMeshAgent agent, Vector3 target)
        {
            NavMeshPath path = new NavMeshPath();
            if (!agent.CalculatePath(target, path))
            {
                return false;
            }
            return path.status == NavMeshPathStatus.PathComplete;
        }
    }
    public sealed class WaitForSomeFrame : CustomYieldInstruction
    {
        private int remainingFrames;

        public WaitForSomeFrame(int frameCount)
        {
            remainingFrames = Mathf.Max(0, frameCount);
        }

        public override bool keepWaiting => remainingFrames-- > 0;
    }
}
