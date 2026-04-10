![Saneject title slide with the tagline 'Unity.EditorTime.DI' on a dark circuit-pattern background. The image reads 'Inject dependencies in the Unity Editor, before you even press Play,' highlights that it is 100% free and open source, and shows the URL saneject.dev.](images/slides/saneject-01.webp)

![Comparison slide titled 'Cleaner, faster components with fewer moving parts' contrasting a manual Unity component setup with a Saneject-injected version. Red callouts list drawbacks like expensive searches and hidden dependencies, while green callouts highlight edit-time injection, serialized fields, Inspector interface support, and cleaner code.](images/slides/saneject-02.webp)

![Overview diagram titled 'How Saneject works at a glance' showing a Unity scene hierarchy, example C# code, scope bindings, and an Inspector screenshot. It explains that dependencies are resolved in the Unity Editor and written into serialized fields before the game starts.](images/slides/saneject-03.webp)

![Feature comparison table titled 'Why editor-time DI?' comparing traditional runtime dependency injection with Saneject's editor-time approach. Rows cover injection timing, lifecycle, performance, Inspector visibility, debugging, determinism, build stability, flexibility, and testing.](images/slides/saneject-04.webp)

![Slide titled 'Supports multiple injection site types' showing code examples for concrete types, interfaces, fields and properties, collections, method parameters, and plain serializable classes. It explains that Saneject can inject Unity objects as concrete or interface types into several kinds of targets.](images/slides/saneject-05.webp)

![Slide titled 'Serialized interface references in the Inspector' explaining that SerializeInterface makes interface fields and properties serializable and visible in Unity's Inspector. The image includes C# examples alongside an Inspector view listing multiple ability implementations.](images/slides/saneject-06.webp)

![Slide titled 'Flexible binding across scenes, prefabs, and assets' showing code snippets for locating components in scene or prefab hierarchies and loading assets from project folders. Examples include binding from scope descendants, resources, asset folders, direct instances, and custom factory methods.](images/slides/saneject-07.webp)

![Slide titled 'Fine-grained filtering for bindings' with code examples for component filtering, asset filtering, and target qualifiers. It demonstrates restricting bindings by predicates, names, paths, member targets, and inject IDs.](images/slides/saneject-08.webp)

![Slide titled 'Inject into nested serialized classes' showing a MonoBehaviour root class, nested serializable classes, and a Unity Inspector example. It explains that Saneject can recursively inject dependencies into nested serializable classes at any depth.](images/slides/saneject-09.webp)

![Slide titled 'Batch inject your entire project in one click' showing a Saneject batch injector editor window beside Unity console output. It explains that injection can run across scenes and prefabs with scoped logs and a final summary.](images/slides/saneject-10.webp)

![Slide titled 'Comprehensive logging & validation' showing a Unity console filled with Saneject warnings, errors, and summary lines. It emphasizes that binding and dependency issues are detected, reported, and summarized in a single editor-time injection pass.](images/slides/saneject-11.webp)

![Slide titled 'Bridge scenes & prefabs with runtime proxies' explaining a four-step example where a prefab needs a scene-only GameManager reference. Diagrams and Inspector screenshots show Saneject injecting a runtime proxy and swapping it for the real instance at startup.](images/slides/saneject-12.webp)

![Slide titled 'Quality-of-life editor features' listing many Unity editor conveniences provided by Saneject. The badges mention features such as native UI, show or hide injected fields, batch injection, filtered logs, ping from logs, auto proxy creation, analyzer support, configurable logging, and validated interface drag-and-drop.](images/slides/saneject-13.webp)