using MonoTouch.UIKit;

namespace System.Windows.Interactivity
{
    public abstract class Behavior<T> : Behavior where T : UIView
    {
        private UIView _associatedObject;

        protected T AssociatedObject
        {
            get
            {
                return (T)_associatedObject;
            }
        }

        protected Behavior(UIView view) : base(typeof(T))
        {
            _associatedObject = view;
            if (_associatedObject == null)
                throw new Exception("Objet associé non trouvé au behavior");
            OnAttached();
        }
    }

    public abstract class Behavior : UIView
    {
        private Type _associatedType;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            this.OnDetaching();
        }

        public abstract void OnAttached();

        public abstract void OnDetaching();

        protected Behavior(Type associatedType)
        {
            _associatedType = associatedType;
        }
    }
}