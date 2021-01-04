namespace fsharp_api.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open System.Text.RegularExpressions
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open fsharp_api

module Payments =
    type CheckNumber = CheckNumber of int
    type CardType = CardType of string
    type CardNumber = CardNumber of int

    type PaymentMethod =
    | Cash
    | Check of CheckNumber
    | Card of CardType * CardNumber

    let printPayment method =
        match method with
        | Cash -> printfn "Paid in cash"
        | Check checkNo -> printfn "Paid by check: %A" checkNo
        | Card (cardType, cardNo) -> printfn "Paid with %A %A" cardType cardNo

module Entities =
    type EmailAddress = EmailAddress of string
    type PhoneNumber = PhoneNumber of string
    type FaxNumber = FaxNumber of string
    type VerifiedEmail = VerifiedEmail of EmailAddress
    type VerificationHash = VerificationHash of string
    type VerificationService = (EmailAddress * VerificationHash) -> VerifiedEmail option
    type CredentialNumber = CredentialNumber of string
    type CredentialExpiration = CredentialExpiration of DateTime

    type DatabaseInfo =
        {
            CreatedAt: DateTime
            ModifiedBy: int
            UpdatedAt: DateTime
        }

    type PersonalName =
        {
            FirstName: string
            LastName: string
            MiddleName: string option
        }

    type EmailContactInfo =
        | Unverified of EmailAddress option
        | Verified of VerifiedEmail

    type Address =
        {
            City: string
            State: string
            Street1: string
            Street2: string option
            ZipCode: string
        }

    type TeacherCredentialInfo =
        {
            CredentialArea: string option
            CredentialExpiration: CredentialExpiration option
            CredentialNumber: CredentialNumber option
            IsTeacher: bool option
        }
    type UniversalContact =
        {
            Birthday: DateTime option
            Address: Address
            DatabaseInfo: DatabaseInfo
            Email: EmailContactInfo list
            Id: int
            Name: PersonalName
            PhoneNumber: PhoneNumber list
            TeacherCredentials: TeacherCredentialInfo option
        }

    // constructor functions
    let createEmailAddress (s : string) : EmailAddress option = 
        if Regex.IsMatch(s, @"^\S+@\S+\.\S+$")
            then Some (EmailAddress s)
            else None

    let createEmailInfo (email : string) : EmailContactInfo =
        match email with
        | s -> (Unverified (createEmailAddress s))

    let createPhoneNumber (s : string) : PhoneNumber option =
        if s.Length = 10
            then Some (PhoneNumber s)
            else None


[<ApiController>]
[<Route("[controller]")>]
type UserController (logger : ILogger<UserController>) =
    inherit ControllerBase()

    let (users: Entities.UniversalContact list) = 
        [
            { 
                Birthday = None
                Address =
                    {
                        Street1 = "Dingo Drive"
                        Street2 = None
                        City = "Seattle"
                        State = "WA"
                        ZipCode = "91809"
                    }
                DatabaseInfo =
                    {
                        CreatedAt = DateTime.Now
                        UpdatedAt = DateTime.Now
                        ModifiedBy = 1
                    }
                Email = [ Entities.createEmailInfo("bob@gmail.com") ]
                Id = 1
                Name =
                    {
                        FirstName = "Bob"
                        LastName = "Ross"
                        MiddleName = None
                    }
                PhoneNumber = []
                TeacherCredentials = None
            };
            { 
                Birthday = None
                Address =
                    {
                        Street1 = "Flamingo Way"
                        Street2 = None
                        City = "Seattle"
                        State = "WA"
                        ZipCode = "91809"
                    }
                DatabaseInfo =
                    {
                        CreatedAt = DateTime.Now
                        UpdatedAt = DateTime.Now
                        ModifiedBy = 1
                    }
                Email = 
                    [
                        Entities.createEmailInfo("dingo@gmail.com");
                        Entities.createEmailInfo("delightful.dingo@gmail.com");
                    ]
                Id = 2
                Name =
                    {
                        FirstName = "Delightful"
                        LastName = "Dingo"
                        MiddleName = None
                    }
                PhoneNumber = []
                TeacherCredentials = None
            };
        ]

    let getEmail (email: Entities.EmailContactInfo) : string =
        match email with
        | Entities.Verified s -> string s
        | Entities.Unverified s -> 
            match s with
            | Some s -> sprintf "%A" s
            | None -> ""


    [<HttpGet>]
    member __.Get() : ActionResult<Entities.UniversalContact list> =
        users 
        |> ActionResult<Entities.UniversalContact list>

    [<HttpGet("{id}")>] 
    member __.Get(id : int): ActionResult<Entities.UniversalContact list> = 
        users
        |> List.filter (fun (user: Entities.UniversalContact) -> user.Id = id)
        |> ActionResult<Entities.UniversalContact list>

    [<HttpGet("emailtest")>] 
    member __.GetEmails(id : int): ActionResult<string> = 
        let email = getEmail(Entities.createEmailInfo("dingo@gmail.com"))
        ActionResult<string> email

