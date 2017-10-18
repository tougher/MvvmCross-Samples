using System.Threading.Tasks;
using Acr.UserDialogs;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.ViewModels;
using StarWarsSample.Core.Models;
using StarWarsSample.Core.MvxInteraction;
using StarWarsSample.Core.ViewModelResults;

namespace StarWarsSample.Core.ViewModels
{
	public class PersonViewModel : BaseViewModel<Person, DestructionResult<Person>>
	{
		private readonly IUserDialogs _userDialogs;
		private readonly IMvxNavigationService _navigationService;

		public PersonViewModel(
			IUserDialogs userDialogs, IMvxNavigationService navigationService)
		{
			_userDialogs = userDialogs;
			_navigationService = navigationService;

			DestroyPersonCommand = new MvxAsyncCommand(DestroyPerson);
		}

		// MvvmCross Lifecycle
		public override void Prepare(Person parameter)
		{
			Person = parameter;
		}

		// MVVM Properties
		private Person _person;
		public Person Person
		{
			get
			{
				return _person;
			}
			set
			{
				_person = value;
				RaisePropertyChanged(() => Person);
			}
		}

		public MvxInteraction<DestructionAction> Interaction { get; set; } = new MvxInteraction<DestructionAction>();

		// MVVM Commands
		public IMvxCommand DestroyPersonCommand { get; private set; }

		// Private methods

		private async Task DestroyPerson()
		{
			var destroy = await _userDialogs.ConfirmAsync(new ConfirmConfig
			{
				Title = "Destroy Person",
				Message = "Sir, are you sure you want to destroy this person?",
				OkText = "YES",
				CancelText = "No"
			});

			if (!destroy)
				return;

			var request = new DestructionAction
			{
				OnDestroyed = () => _navigationService.Close(this,
					new DestructionResult<Person>
					{
						Entity = Person,
						Destroyed = true
					})
			};

			Interaction.Raise(request);
		}
	}
}
