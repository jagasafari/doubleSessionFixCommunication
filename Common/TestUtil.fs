module Common.TestUtil

let cast3TestData data =
    let castTuple (x, y, z) = [|x:>obj; y:>obj; z:>obj|]
    Seq.map castTuple data
