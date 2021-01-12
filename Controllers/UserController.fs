namespace fsharp_api.Controllers

open System
open System.Text.RegularExpressions
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open FSharp.Data.Sql

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

    let getEmail (email: EmailContactInfo) : string =
        match email with
        | Verified s -> string s
        | Unverified None -> ""
        | Unverified (Some (EmailAddress s)) -> s

module db =
    [<Literal>]
    let ConnString = "Server=localhost;Database=test_users;User=dingo;Password=dingo"

    [<Literal>]
    let DbVendor = Common.DatabaseProviderTypes.MYSQL

    [<Literal>]
    let IndivAmount = 1000

    [<Literal>]
    let UseOptTypes = true

    [<Literal>]
    let ResPath = __SOURCE_DIRECTORY__ + @"./../bin/Debug/netcoreapp3.1"

    type Sql = SqlDataProvider<
                    ResolutionPath = ResPath,
                    IndividualsAmount = IndivAmount,
                    UseOptionTypes = UseOptTypes
                >
    let ctx = Sql.GetDataContext()

[<ApiController>]
[<Route("[controller]")>]
type UserController (logger : ILogger<UserController>) =
    inherit ControllerBase()

    // [<HttpGet>]
    // member __.Get() : ActionResult<Entities.UniversalContact list> =
    //     ctx.user
    //     |> Seq.toList
    //     |> ActionResult<Entities.UniversalContact list>

    // [<HttpGet("{id}")>] 
    // member __.Get(id : int): ActionResult<Entities.UniversalContact list> = 
    //     ctx.user
    //     |> List.filter (fun (user: Entities.UniversalContact) -> user.Id = id)
    //     |> ActionResult<Entities.UniversalContact list>

    [<HttpGet("emailtest")>] 
    member __.GetEmails(id : int): ActionResult<string> = 
        let email = Entities.getEmail(Entities.createEmailInfo("dingo@gmail.com"))
        ActionResult<string> email

