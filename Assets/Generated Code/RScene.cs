namespace ABXY.AssetLink
{
    public class RScene
    {
        public class Bins
        {
            /// <summary>Get all scene components matching RScene\Bins</summary>
            public static UnityEngine.Component[] GetAll()
            {
                return ABXY.AssetLink.Internal.RealtimeResourceContainer.GetSceneComponents("30618004-5e28-424a-8253-88fccc032d5a");
            }
            /// <summary>Get all scene components matching RScene\Bins of type T</summary>
            public static T[] GetAll<T>() where T : UnityEngine.Component
            {
                UnityEngine.Component[] components = GetAll();
                System.Collections.Generic.List<T> castComponents = new System.Collections.Generic.List<T>();
                foreach (UnityEngine.Component component in components)
                {
                    T castComponent = (T)component;
                    if (castComponent != null)
                        castComponents.Add(castComponent);
                }
                return castComponents.ToArray();
            }
            /// <summary>Get a scene component matching RScene\Bins of type T, if one exists</summary>
            public static T Get<T>() where T : UnityEngine.Component
            {
                T[] result = GetAll<T>();
                return result.Length != 0 ? result[0] : null;
            }
            /// <summary>Get all scene components matching RScene\Bins and the given selector predicate</summary>
            public static UnityEngine.Component GetWhere(System.Predicate<UnityEngine.Component> selector)
            {
                UnityEngine.Component[] result = GetAllWhere(selector);
                return result.Length != 0 ? result[0] : null;
            }
            /// <summary>Get a scene component matching RScene\Bins and the given selector predicate</summary>
            public static T GetWhere<T>(System.Predicate<T> selector) where T : UnityEngine.Component
            {
                T[] result = GetAllWhere<T>(selector);
                return result.Length != 0 ? result[0] : null;
            }
            /// <summary>Get a scene components matching RScene\Bins</summary>
            public static UnityEngine.Component Get()
            {
                UnityEngine.Component[] result = GetAll();
                return result.Length != 0 ? result[0] : null;
            }
            /// <summary>Get all scene components matching RScene\Bins and the given selector predicate</summary>
            public static UnityEngine.Component[] GetAllWhere(System.Predicate<UnityEngine.Component> selector)
            {
                System.Collections.Generic.List<UnityEngine.Component> components = new System.Collections.Generic.List<UnityEngine.Component>();
                components.AddRange(GetAll());
                return components.FindAll(selector).ToArray();
            }
            /// <summary>Get all scene components matching RScene\Bins and the given selector predicate</summary>
            public static T[] GetAllWhere<T>(System.Predicate<T> selector) where T : UnityEngine.Component
            {
                System.Collections.Generic.List<T> components = new System.Collections.Generic.List<T>();
                components.AddRange(GetAll<T>());
                return components.FindAll(selector).ToArray();
            }
        }
    }
}
